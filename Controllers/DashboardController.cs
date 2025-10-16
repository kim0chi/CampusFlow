using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEnrollmentWorkflowService _workflowService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        ApplicationDbContext context,
        IEnrollmentWorkflowService workflowService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _workflowService = workflowService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user!);
        var primaryRole = roles.FirstOrDefault() ?? "Student";

        var viewModel = new DashboardViewModel
        {
            RoleName = primaryRole
        };

        if (primaryRole == "Student")
        {
            // Show enrollment batches instead of individual enrollments
            var enrollmentBatches = await _context.EnrollmentBatches
                .Include(eb => eb.Enrollments)
                .ThenInclude(e => e.Course)
                .Include(eb => eb.ApprovalSteps)
                .Where(eb => eb.StudentId == user!.Id)
                .OrderByDescending(eb => eb.SubmittedDate)
                .Take(10)
                .ToListAsync();

            ViewBag.EnrollmentBatches = enrollmentBatches;
        }
        else if (primaryRole != "Admin")
        {
            var pendingBatches = await _workflowService.GetPendingEnrollmentBatchesForRoleAsync(primaryRole);
            ViewBag.PendingBatches = pendingBatches;
            ViewBag.PendingCount = pendingBatches.Count;

            // Get department statistics
            var stepType = GetStepTypeFromRole(primaryRole);
            if (stepType.HasValue)
            {
                ViewBag.TotalReviewed = await _context.ApprovalSteps
                    .Where(a => a.StepType == stepType.Value && a.Status != ApprovalStatus.Pending)
                    .CountAsync();

                ViewBag.TotalApproved = await _context.ApprovalSteps
                    .Where(a => a.StepType == stepType.Value && a.Status == ApprovalStatus.Approved)
                    .CountAsync();

                ViewBag.TotalRejected = await _context.ApprovalSteps
                    .Where(a => a.StepType == stepType.Value && a.Status == ApprovalStatus.Rejected)
                    .CountAsync();

                ViewBag.PendingThisWeek = await _context.EnrollmentBatches
                    .Where(eb => eb.SubmittedDate >= DateTime.UtcNow.AddDays(-7))
                    .Include(eb => eb.ApprovalSteps)
                    .CountAsync(eb => eb.ApprovalSteps.Any(a => a.StepType == stepType.Value && a.Status == ApprovalStatus.Pending));

                // Recent activity (last 5 processed)
                ViewBag.RecentActivity = await _context.ApprovalSteps
                    .Include(a => a.EnrollmentBatch)
                    .ThenInclude(eb => eb.Student)
                    .Include(a => a.EnrollmentBatch)
                    .ThenInclude(eb => eb.Enrollments)
                    .ThenInclude(e => e.Course)
                    .Where(a => a.StepType == stepType.Value && a.Status != ApprovalStatus.Pending)
                    .OrderByDescending(a => a.ActionDate)
                    .Take(5)
                    .ToListAsync();
            }
        }
        else
        {
            // Admin dashboard - show overall statistics
            ViewBag.TotalEnrollmentBatches = await _context.EnrollmentBatches.CountAsync();
            ViewBag.PendingEnrollmentBatches = await _context.EnrollmentBatches
                .CountAsync(eb => eb.Status != EnrollmentStatus.Enrolled && eb.Status != EnrollmentStatus.Rejected);
            ViewBag.CompletedEnrollmentBatches = await _context.EnrollmentBatches
                .CountAsync(eb => eb.Status == EnrollmentStatus.Enrolled);
            ViewBag.TotalCourses = await _context.Courses.CountAsync(c => c.IsActive);
        }

        return View(viewModel);
    }

    [Authorize(Roles = "Dean,Accounting,SAO,Library,Records")]
    public async Task<IActionResult> PendingApprovals()
    {
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user!);
        var primaryRole = roles.FirstOrDefault();

        if (string.IsNullOrEmpty(primaryRole))
        {
            return Forbid();
        }

        var enrollmentBatches = await _workflowService.GetPendingEnrollmentBatchesForRoleAsync(primaryRole);

        ViewBag.RoleName = primaryRole;
        return View(enrollmentBatches);
    }

    [Authorize(Roles = "Dean,Accounting,SAO,Library,Records")]
    public async Task<IActionResult> ApprovalDetails(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var enrollmentBatch = await _context.EnrollmentBatches
            .Include(eb => eb.Student)
            .ThenInclude(s => s.Department)
            .Include(eb => eb.Enrollments)
            .ThenInclude(e => e.Course)
            .Include(eb => eb.ApprovalSteps)
            .ThenInclude(a => a.Approver)
            .FirstOrDefaultAsync(eb => eb.Id == id);

        if (enrollmentBatch == null)
        {
            return NotFound();
        }

        // Check if user can approve this enrollment batch
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user!);
        var primaryRole = roles.FirstOrDefault();

        var canApprove = await _workflowService.CanUserApproveAsync(enrollmentBatch.Id, user!.Id, primaryRole!);
        ViewBag.CanApprove = canApprove;
        ViewBag.UserRole = primaryRole;

        // Load payment/promissory note information for Accounting role
        var enrollmentPayment = await _context.EnrollmentPayments
            .Include(ep => ep.Payment)
            .Include(ep => ep.PromissoryNote)
                .ThenInclude(pn => pn!.ReviewedByCampusDirector)
            .FirstOrDefaultAsync(ep => ep.EnrollmentBatchId == enrollmentBatch.Id);

        ViewBag.EnrollmentPayment = enrollmentPayment;

        return View(enrollmentBatch);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Dean,Accounting,SAO,Library,Records")]
    public async Task<IActionResult> ProcessApproval(ApprovalViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var roles = await _userManager.GetRolesAsync(user!);
                var primaryRole = roles.FirstOrDefault();

                if (string.IsNullOrEmpty(primaryRole))
                {
                    return Forbid();
                }

                // Get the step type from role
                var stepType = GetStepTypeFromRole(primaryRole);
                if (!stepType.HasValue)
                {
                    return BadRequest("Invalid role for approval.");
                }

                await _workflowService.ProcessApprovalAsync(
                    model.EnrollmentBatchId,
                    stepType.Value,
                    user!.Id,
                    model.IsApproved,
                    model.Comments);

                TempData["Success"] = model.IsApproved
                    ? "Enrollment batch approved successfully!"
                    : "Enrollment batch rejected.";

                return RedirectToAction(nameof(PendingApprovals));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ApprovalDetails), new { id = model.EnrollmentBatchId });
            }
        }

        return RedirectToAction(nameof(ApprovalDetails), new { id = model.EnrollmentBatchId });
    }

    [Authorize(Roles = "Dean,Accounting,SAO,Library,Records")]
    public async Task<IActionResult> ApprovalHistory()
    {
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user!);
        var primaryRole = roles.FirstOrDefault();

        if (string.IsNullOrEmpty(primaryRole))
        {
            return Forbid();
        }

        var stepType = GetStepTypeFromRole(primaryRole);
        if (!stepType.HasValue)
        {
            return BadRequest("Invalid role for viewing approval history.");
        }

        // Get all approval steps processed by this role
        var approvalHistory = await _context.ApprovalSteps
            .Include(a => a.EnrollmentBatch)
            .ThenInclude(eb => eb.Student)
            .Include(a => a.EnrollmentBatch)
            .ThenInclude(eb => eb.Enrollments)
            .ThenInclude(e => e.Course)
            .Include(a => a.Approver)
            .Where(a => a.StepType == stepType.Value && a.Status != ApprovalStatus.Pending)
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync();

        ViewBag.RoleName = primaryRole;
        ViewBag.TotalProcessed = approvalHistory.Count;
        ViewBag.TotalApproved = approvalHistory.Count(a => a.Status == ApprovalStatus.Approved);
        ViewBag.TotalRejected = approvalHistory.Count(a => a.Status == ApprovalStatus.Rejected);

        return View(approvalHistory);
    }

    private ApprovalStepType? GetStepTypeFromRole(string roleName)
    {
        return roleName switch
        {
            "Dean" => ApprovalStepType.Dean,
            "Accounting" => ApprovalStepType.Accounting,
            "SAO" => ApprovalStepType.SAO,
            "Library" => ApprovalStepType.Library,
            "Records" => ApprovalStepType.Records,
            _ => null
        };
    }
}
