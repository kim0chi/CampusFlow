using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize(Roles = "Student")]
public class EnrollmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEnrollmentWorkflowService _workflowService;
    private readonly UserManager<ApplicationUser> _userManager;

    public EnrollmentController(
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
        var userId = _userManager.GetUserId(User);
        var enrollmentBatches = await _context.EnrollmentBatches
            .Include(eb => eb.Enrollments)
            .ThenInclude(e => e.Course)
            .Include(eb => eb.ApprovalSteps)
            .Where(eb => eb.StudentId == userId)
            .OrderByDescending(eb => eb.SubmittedDate)
            .ToListAsync();

        return View(enrollmentBatches);
    }

    public async Task<IActionResult> Create(int? id)
    {
        var userId = _userManager.GetUserId(User);
        var student = await _userManager.GetUserAsync(User);

        var courses = await _context.Courses
            .Where(c => c.IsActive)
            .Include(c => c.PrerequisiteRelations)
            .ThenInclude(pr => pr.PrerequisiteCourse)
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();

        // Get student's completed courses
        var completedCourseIds = await _context.Enrollments
            .Where(e => e.StudentId == userId && e.Status == Models.Enums.EnrollmentStatus.Completed)
            .Select(e => e.CourseId)
            .ToListAsync();

        ViewBag.Courses = courses;
        ViewBag.CompletedCourseIds = completedCourseIds;
        ViewBag.StudentYearLevel = student?.YearOfStudy;
        ViewBag.SelectedCourseId = id;

        // Get active academic years
        var academicYears = await _context.AcademicYears
            .Where(ay => ay.IsActive)
            .OrderByDescending(ay => ay.Year)
            .ToListAsync();

        ViewBag.AcademicYears = academicYears;

        // Get current academic year or first active year
        var currentAcademicYear = academicYears.FirstOrDefault(ay => ay.IsCurrentYear)
            ?? academicYears.FirstOrDefault();

        // Default semester based on current month
        var currentMonth = DateTime.Now.Month;
        string defaultSemester;

        if (currentMonth >= 8 && currentMonth <= 12) // August-December: 1st Semester
        {
            defaultSemester = "1st Semester";
        }
        else if (currentMonth >= 1 && currentMonth <= 5) // January-May: 2nd Semester
        {
            defaultSemester = "2nd Semester";
        }
        else // June-July: Summer
        {
            defaultSemester = "Summer";
        }

        ViewBag.DefaultSemester = defaultSemester;
        ViewBag.DefaultAcademicYear = currentAcademicYear?.Year;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BulkEnrollmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _workflowService.CreateEnrollmentBatchAsync(
                    userId!,
                    model.CourseIds,
                    model.Semester,
                    model.AcademicYear,
                    model.Comments);

                TempData["Success"] = $"Enrollment request for {model.CourseIds.Length} course(s) submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        // Reload data for view
        var student = await _userManager.GetUserAsync(User);
        var courses = await _context.Courses
            .Where(c => c.IsActive)
            .Include(c => c.PrerequisiteRelations)
            .ThenInclude(pr => pr.PrerequisiteCourse)
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();

        var userId2 = _userManager.GetUserId(User);
        var completedCourseIds = await _context.Enrollments
            .Where(e => e.StudentId == userId2 && e.Status == Models.Enums.EnrollmentStatus.Completed)
            .Select(e => e.CourseId)
            .ToListAsync();

        ViewBag.Courses = courses;
        ViewBag.CompletedCourseIds = completedCourseIds;
        ViewBag.StudentYearLevel = student?.YearOfStudy;
        ViewBag.DefaultSemester = model.Semester;
        ViewBag.DefaultAcademicYear = model.AcademicYear;

        return View(model);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var enrollmentBatch = await _context.EnrollmentBatches
            .Include(eb => eb.Enrollments)
            .ThenInclude(e => e.Course)
            .Include(eb => eb.ApprovalSteps)
            .ThenInclude(a => a.Approver)
            .FirstOrDefaultAsync(eb => eb.Id == id && eb.StudentId == userId);

        if (enrollmentBatch == null)
        {
            return NotFound();
        }

        return View(enrollmentBatch);
    }
}
