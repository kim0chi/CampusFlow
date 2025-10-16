using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Services;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Controllers;

[Authorize(Roles = "Student")]
public class StudentAccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStudentAccountService _accountService;

    public StudentAccountController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IStudentAccountService accountService)
    {
        _context = context;
        _userManager = userManager;
        _accountService = accountService;
    }

    // View Account Balance and Fees
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var currentSemester = "1st Semester";
        var currentAcademicYear = "2024-2025";

        var account = await _accountService.GetOrCreateStudentAccountAsync(userId!, currentSemester, currentAcademicYear);
        var fees = await _accountService.GetStudentFeesAsync(userId!, currentSemester, currentAcademicYear);
        var payments = await _accountService.GetStudentPaymentsAsync(userId!, currentSemester, currentAcademicYear);

        // Get enrollments awaiting payment
        var enrollmentsAwaitingPayment = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == userId && e.Status == EnrollmentStatus.AwaitingPayment)
            .ToListAsync();

        ViewBag.Account = account;
        ViewBag.Fees = fees;
        ViewBag.Payments = payments;
        ViewBag.EnrollmentsAwaitingPayment = enrollmentsAwaitingPayment;
        ViewBag.RequiredPayment = account.CurrentBalance * 0.30m;

        return View();
    }

    // Make Payment
    public async Task<IActionResult> MakePayment(int? enrollmentId)
    {
        var userId = _userManager.GetUserId(User);
        var currentSemester = "1st Semester";
        var currentAcademicYear = "2024-2025";

        var balance = await _accountService.GetStudentBalanceAsync(userId!, currentSemester, currentAcademicYear);
        var requiredPayment = balance * 0.30m;

        ViewBag.Balance = balance;
        ViewBag.RequiredPayment = requiredPayment;
        ViewBag.EnrollmentId = enrollmentId;
        ViewBag.PaymentMethods = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>().ToList();

        // If enrollment batch specified, get batch details
        if (enrollmentId.HasValue)
        {
            var enrollmentBatch = await _context.EnrollmentBatches
                .Include(eb => eb.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(eb => eb.Id == enrollmentId.Value && eb.StudentId == userId);

            ViewBag.EnrollmentBatch = enrollmentBatch;
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakePayment(decimal amount, PaymentMethod paymentMethod, int? enrollmentId)
    {
        var userId = _userManager.GetUserId(User);
        var currentSemester = "1st Semester";
        var currentAcademicYear = "2024-2025";

        if (amount <= 0)
        {
            ModelState.AddModelError("", "Payment amount must be greater than zero.");
            return await MakePayment(enrollmentId);
        }

        // Get or create student account
        var account = await _accountService.GetOrCreateStudentAccountAsync(userId!, currentSemester, currentAcademicYear);
        var balance = account.CurrentBalance;

        // Generate reference number
        var referenceNumber = GenerateReferenceNumber();

        // Create payment record
        var payment = new Payment
        {
            StudentId = userId!,
            StudentAccountId = account.Id,
            Amount = amount,
            PaymentMethod = paymentMethod,
            ReferenceNumber = referenceNumber,
            PaymentDate = DateTime.UtcNow,
            Semester = currentSemester,
            AcademicYear = currentAcademicYear,
            Remarks = "Student self-payment"
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Update account balances
        await _accountService.UpdateAccountBalancesAsync(userId!, currentSemester, currentAcademicYear);

        // If payment is for specific enrollment batch, create EnrollmentPayment
        if (enrollmentId.HasValue)
        {
            var enrollmentBatch = await _context.EnrollmentBatches
                .Include(eb => eb.Enrollments)
                .FirstOrDefaultAsync(eb => eb.Id == enrollmentId.Value && eb.StudentId == userId);

            if (enrollmentBatch != null && enrollmentBatch.Status == EnrollmentStatus.AwaitingPayment)
            {
                var percentage = await _accountService.CalculatePaymentPercentageAsync(amount, balance);
                var meetsRequirement = await _accountService.MeetsPaymentRequirementAsync(percentage);

                var enrollmentPayment = new EnrollmentPayment
                {
                    EnrollmentBatchId = enrollmentBatch.Id,
                    PaymentId = payment.Id,
                    AmountPaid = amount,
                    StudentBalance = balance,
                    PaymentPercentage = percentage,
                    MeetsRequirement = meetsRequirement,
                    UsingPromissoryNote = false
                };

                _context.EnrollmentPayments.Add(enrollmentPayment);

                // If payment meets requirement, move to Accounting Review
                if (meetsRequirement)
                {
                    enrollmentBatch.Status = EnrollmentStatus.AccountingReview;

                    // Update all individual enrollments in the batch
                    foreach (var enrollment in enrollmentBatch.Enrollments)
                    {
                        enrollment.Status = EnrollmentStatus.AccountingReview;
                    }

                    // Create Accounting approval step
                    var accountingStep = new ApprovalStep
                    {
                        EnrollmentBatchId = enrollmentBatch.Id,
                        StepType = ApprovalStepType.Accounting,
                        Status = ApprovalStatus.Pending,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.ApprovalSteps.Add(accountingStep);
                }

                await _context.SaveChangesAsync();
            }
        }

        TempData["Success"] = $"Payment of ₱{amount:N2} processed successfully. Reference: {referenceNumber}";
        return RedirectToAction(nameof(PaymentReceipt), new { id = payment.Id });
    }

    // View Payment Receipt
    public async Task<IActionResult> PaymentReceipt(int id)
    {
        var userId = _userManager.GetUserId(User);

        var payment = await _context.Payments
            .Include(p => p.Student)
            .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == userId);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    private string GenerateReferenceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"PAY-{timestamp}-{random}";
    }
}
