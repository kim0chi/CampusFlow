using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize(Roles = "Cashier,Admin")]
public class CashierController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStudentAccountService _accountService;

    public CashierController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IStudentAccountService accountService)
    {
        _context = context;
        _userManager = userManager;
        _accountService = accountService;
    }

    // Dashboard - Shows students with their balances
    public async Task<IActionResult> Index()
    {
        var currentSemester = "1st Semester";
        var currentAcademicYear = "2024-2025";

        // Get all students
        var students = await _userManager.GetUsersInRoleAsync("Student");

        var studentBalances = new List<(ApplicationUser Student, decimal Balance, StudentAccount? Account)>();

        foreach (var student in students)
        {
            var account = await _context.StudentAccounts
                .FirstOrDefaultAsync(sa => sa.StudentId == student.Id
                    && sa.Semester == currentSemester
                    && sa.AcademicYear == currentAcademicYear);

            var balance = account?.CurrentBalance ?? 0;
            studentBalances.Add((student, balance, account));
        }

        ViewBag.CurrentSemester = currentSemester;
        ViewBag.CurrentAcademicYear = currentAcademicYear;
        ViewBag.StudentBalances = studentBalances.OrderBy(s => s.Student.LastName).ToList();

        return View();
    }

    // Process Payment - Form to process a student payment
    public async Task<IActionResult> ProcessPayment(string? studentId)
    {
        var students = await _userManager.GetUsersInRoleAsync("Student");
        ViewBag.Students = students.OrderBy(s => s.LastName).ToList();
        ViewBag.PaymentMethods = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>().ToList();

        ViewBag.SelectedStudentId = studentId;

        if (!string.IsNullOrEmpty(studentId))
        {
            var balance = await _accountService.GetStudentBalanceAsync(studentId, "1st Semester", "2024-2025");
            ViewBag.StudentBalance = balance;
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayment(string studentId, decimal amount, PaymentMethod paymentMethod, string? remarks)
    {
        if (amount <= 0)
        {
            ModelState.AddModelError("", "Payment amount must be greater than zero.");
            return await ProcessPayment(studentId);
        }

        var currentSemester = "1st Semester";
        var currentAcademicYear = "2024-2025";

        // Get or create student account
        var account = await _accountService.GetOrCreateStudentAccountAsync(studentId, currentSemester, currentAcademicYear);

        // Generate reference number
        var referenceNumber = GenerateReferenceNumber();

        // Create payment record
        var payment = new Payment
        {
            StudentId = studentId,
            StudentAccountId = account.Id,
            Amount = amount,
            PaymentMethod = paymentMethod,
            ReferenceNumber = referenceNumber,
            PaymentDate = DateTime.UtcNow,
            Semester = currentSemester,
            AcademicYear = currentAcademicYear,
            ProcessedByCashierId = _userManager.GetUserId(User),
            Remarks = remarks
        };

        _context.Payments.Add(payment);

        // Update account balances
        await _accountService.UpdateAccountBalancesAsync(studentId, currentSemester, currentAcademicYear);

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Payment of ₱{amount:N2} processed successfully. Reference: {referenceNumber}";
        return RedirectToAction(nameof(PaymentReceipt), new { id = payment.Id });
    }

    // View Payment Receipt
    public async Task<IActionResult> PaymentReceipt(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.ProcessedByCashier)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // View Payment History
    public async Task<IActionResult> PaymentHistory(string? studentId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Payments
            .Include(p => p.Student)
            .Include(p => p.ProcessedByCashier)
            .AsQueryable();

        if (!string.IsNullOrEmpty(studentId))
        {
            query = query.Where(p => p.StudentId == studentId);
        }

        if (startDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate <= endDate.Value);
        }

        var payments = await query
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        var students = await _userManager.GetUsersInRoleAsync("Student");
        ViewBag.Students = students.OrderBy(s => s.LastName).ToList();
        ViewBag.SelectedStudentId = studentId;
        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;

        return View(payments);
    }

    // View Student Account Details
    public async Task<IActionResult> StudentAccount(string id)
    {
        var student = await _userManager.FindByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var currentSemester = "1st Semester";
        var currentAcademicYear = "2024-2025";

        var account = await _accountService.GetOrCreateStudentAccountAsync(id, currentSemester, currentAcademicYear);
        var fees = await _accountService.GetStudentFeesAsync(id, currentSemester, currentAcademicYear);
        var payments = await _accountService.GetStudentPaymentsAsync(id, currentSemester, currentAcademicYear);

        ViewBag.Student = student;
        ViewBag.Account = account;
        ViewBag.Fees = fees;
        ViewBag.Payments = payments;

        return View();
    }

    private string GenerateReferenceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"PAY-{timestamp}-{random}";
    }
}
