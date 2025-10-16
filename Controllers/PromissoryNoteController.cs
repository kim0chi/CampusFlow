using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class PromissoryNoteController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStudentAccountService _accountService;

    public PromissoryNoteController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IStudentAccountService accountService)
    {
        _context = context;
        _userManager = userManager;
        _accountService = accountService;
    }

    // Student: Submit Promissory Note
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit(int enrollmentId)
    {
        var enrollmentBatch = await _context.EnrollmentBatches
            .Include(eb => eb.Enrollments)
            .ThenInclude(e => e.Course)
            .Include(eb => eb.Student)
            .FirstOrDefaultAsync(eb => eb.Id == enrollmentId);

        if (enrollmentBatch == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (enrollmentBatch.StudentId != userId)
        {
            return Forbid();
        }

        // Check if enrollment batch is awaiting payment
        if (enrollmentBatch.Status != EnrollmentStatus.AwaitingPayment)
        {
            TempData["Error"] = "This enrollment batch is not awaiting payment.";
            return RedirectToAction("Details", "Enrollment", new { id = enrollmentId });
        }

        // Check if promissory note already exists
        var existingNote = await _context.PromissoryNotes
            .FirstOrDefaultAsync(pn => pn.EnrollmentBatchId == enrollmentId);

        if (existingNote != null)
        {
            TempData["Error"] = "A promissory note has already been submitted for this enrollment batch.";
            return RedirectToAction("Details", "Enrollment", new { id = enrollmentId });
        }

        // Get student balance
        var balance = await _accountService.GetStudentBalanceAsync(userId!, "1st Semester", "2024-2025");

        ViewBag.EnrollmentBatch = enrollmentBatch;
        ViewBag.Balance = balance;
        ViewBag.RequiredPayment = balance * 0.30m; // 30%

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit(int enrollmentId, decimal amountCovered, string reason, DateTime repaymentDeadline)
    {
        var enrollmentBatch = await _context.EnrollmentBatches
            .Include(eb => eb.Enrollments)
            .FirstOrDefaultAsync(eb => eb.Id == enrollmentId);

        if (enrollmentBatch == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (enrollmentBatch.StudentId != userId)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            ModelState.AddModelError("", "Please provide a reason for the promissory note.");
            return await Submit(enrollmentId);
        }

        if (repaymentDeadline <= DateTime.UtcNow)
        {
            ModelState.AddModelError("", "Repayment deadline must be in the future.");
            return await Submit(enrollmentId);
        }

        // Create promissory note
        var promissoryNote = new PromissoryNote
        {
            EnrollmentBatchId = enrollmentId,
            StudentId = userId!,
            AmountCovered = amountCovered,
            Reason = reason,
            RepaymentDeadline = repaymentDeadline,
            Status = PromissoryNoteStatus.Pending,
            SubmittedDate = DateTime.UtcNow
        };

        _context.PromissoryNotes.Add(promissoryNote);

        // Update enrollment batch status
        enrollmentBatch.Status = EnrollmentStatus.CampusDirectorReview;

        // Update all individual enrollments in the batch
        foreach (var enrollment in enrollmentBatch.Enrollments)
        {
            enrollment.Status = EnrollmentStatus.CampusDirectorReview;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Promissory note submitted successfully and is now awaiting Campus Director review.";
        return RedirectToAction("Details", "Enrollment", new { id = enrollmentId });
    }

    // Campus Director: View Pending Promissory Notes
    [Authorize(Roles = "CampusDirector,Admin")]
    public async Task<IActionResult> PendingReview()
    {
        var pendingNotes = await _context.PromissoryNotes
            .Include(pn => pn.Student)
            .Include(pn => pn.EnrollmentBatch)
                .ThenInclude(eb => eb.Enrollments)
                .ThenInclude(e => e.Course)
            .Where(pn => pn.Status == PromissoryNoteStatus.Pending)
            .OrderBy(pn => pn.SubmittedDate)
            .ToListAsync();

        return View(pendingNotes);
    }

    // Campus Director: Review Promissory Note
    [Authorize(Roles = "CampusDirector,Admin")]
    public async Task<IActionResult> Review(int id)
    {
        var note = await _context.PromissoryNotes
            .Include(pn => pn.Student)
            .Include(pn => pn.EnrollmentBatch)
                .ThenInclude(eb => eb.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(pn => pn.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        // Get student balance
        var balance = await _accountService.GetStudentBalanceAsync(note.StudentId, "1st Semester", "2024-2025");
        ViewBag.Balance = balance;
        ViewBag.RequiredPayment = balance * 0.30m;

        return View(note);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "CampusDirector,Admin")]
    public async Task<IActionResult> Approve(int id, string? comments)
    {
        var note = await _context.PromissoryNotes
            .Include(pn => pn.EnrollmentBatch)
            .ThenInclude(eb => eb.Enrollments)
            .FirstOrDefaultAsync(pn => pn.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        // Update promissory note
        note.Status = PromissoryNoteStatus.Approved;
        note.ReviewedByCampusDirectorId = userId;
        note.ReviewedDate = DateTime.UtcNow;
        note.DirectorComments = comments;

        // Create EnrollmentPayment record
        var balance = await _accountService.GetStudentBalanceAsync(note.StudentId, "1st Semester", "2024-2025");

        var enrollmentPayment = new EnrollmentPayment
        {
            EnrollmentBatchId = note.EnrollmentBatchId,
            PromissoryNoteId = note.Id,
            AmountPaid = 0,
            StudentBalance = balance,
            PaymentPercentage = 0,
            MeetsRequirement = true, // Approved promissory note meets requirement
            UsingPromissoryNote = true
        };

        _context.EnrollmentPayments.Add(enrollmentPayment);

        // Update enrollment batch status to Accounting Review
        note.EnrollmentBatch.Status = EnrollmentStatus.AccountingReview;

        // Update all individual enrollments in the batch
        foreach (var enrollment in note.EnrollmentBatch.Enrollments)
        {
            enrollment.Status = EnrollmentStatus.AccountingReview;
        }

        // Create Accounting approval step
        var accountingStep = new ApprovalStep
        {
            EnrollmentBatchId = note.EnrollmentBatchId,
            StepType = ApprovalStepType.Accounting,
            Status = ApprovalStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };

        _context.ApprovalSteps.Add(accountingStep);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Promissory note approved. Enrollment batch moved to Accounting Review.";
        return RedirectToAction(nameof(PendingReview));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "CampusDirector,Admin")]
    public async Task<IActionResult> Reject(int id, string? comments)
    {
        var note = await _context.PromissoryNotes
            .Include(pn => pn.EnrollmentBatch)
            .ThenInclude(eb => eb.Enrollments)
            .FirstOrDefaultAsync(pn => pn.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(comments))
        {
            ModelState.AddModelError("", "Please provide a reason for rejection.");
            return await Review(id);
        }

        var userId = _userManager.GetUserId(User);

        // Update promissory note
        note.Status = PromissoryNoteStatus.Rejected;
        note.ReviewedByCampusDirectorId = userId;
        note.ReviewedDate = DateTime.UtcNow;
        note.DirectorComments = comments;

        // Update enrollment batch status back to AwaitingPayment
        note.EnrollmentBatch.Status = EnrollmentStatus.AwaitingPayment;

        // Update all individual enrollments in the batch
        foreach (var enrollment in note.EnrollmentBatch.Enrollments)
        {
            enrollment.Status = EnrollmentStatus.AwaitingPayment;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Promissory note rejected. Student must make payment to proceed.";
        return RedirectToAction(nameof(PendingReview));
    }

    // View Promissory Note Status
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Status(int enrollmentId)
    {
        var userId = _userManager.GetUserId(User);

        var note = await _context.PromissoryNotes
            .Include(pn => pn.EnrollmentBatch)
                .ThenInclude(eb => eb.Enrollments)
                .ThenInclude(e => e.Course)
            .Include(pn => pn.ReviewedByCampusDirector)
            .FirstOrDefaultAsync(pn => pn.EnrollmentBatchId == enrollmentId && pn.StudentId == userId);

        if (note == null)
        {
            return NotFound();
        }

        return View(note);
    }
}
