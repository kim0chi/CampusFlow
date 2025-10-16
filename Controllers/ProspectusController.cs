using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Controllers;

[Authorize(Roles = "Student")]
public class ProspectusController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProspectusController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(user.Major))
        {
            ViewBag.Error = "You haven't selected a major yet. Please update your profile to view your prospectus.";
            return View();
        }

        // Find program by major
        var program = await _context.Programs
            .Include(p => p.ProgramCourses)
            .ThenInclude(pc => pc.Course)
            .ThenInclude(c => c.PrerequisiteRelations)
            .ThenInclude(pr => pr.PrerequisiteCourse)
            .FirstOrDefaultAsync(p => p.Name == user.Major && p.IsActive);

        if (program == null)
        {
            ViewBag.Error = $"No program found for major '{user.Major}'. Please contact your advisor.";
            return View();
        }

        // Get all student enrollments
        var allEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == user.Id)
            .ToListAsync();

        // Get student's completed courses (only Completed status counts)
        var completedEnrollments = allEnrollments
            .Where(e => e.Status == EnrollmentStatus.Completed)
            .ToList();

        var completedCourseIds = completedEnrollments.Select(e => e.CourseId).ToList();
        var totalCreditsEarned = completedEnrollments.Sum(e => e.Course.Credits);

        // Get in-progress enrollments (currently taking the course)
        var inProgressEnrollments = allEnrollments
            .Where(e => e.Status == EnrollmentStatus.Enrolled)
            .ToList();

        var inProgressCourseIds = inProgressEnrollments.Select(e => e.CourseId).ToList();

        // Get pending approval enrollments
        var pendingEnrollments = allEnrollments
            .Where(e => e.Status == EnrollmentStatus.Submitted
                || e.Status == EnrollmentStatus.DeanReview
                || e.Status == EnrollmentStatus.AccountingReview
                || e.Status == EnrollmentStatus.SAOReview
                || e.Status == EnrollmentStatus.LibraryReview
                || e.Status == EnrollmentStatus.RecordsReview)
            .ToList();

        var pendingCourseIds = pendingEnrollments.Select(e => e.CourseId).ToList();

        // Get failed enrollments
        var failedEnrollments = allEnrollments
            .Where(e => e.Status == EnrollmentStatus.Failed)
            .ToList();

        var failedCourseIds = failedEnrollments.Select(e => e.CourseId).ToList();

        // Categorize program courses
        var completedProgramCourses = new List<ProgramCourse>();
        var inProgressProgramCourses = new List<ProgramCourse>();
        var pendingProgramCourses = new List<ProgramCourse>();
        var failedProgramCourses = new List<ProgramCourse>();
        var availableProgramCourses = new List<ProgramCourse>();
        var lockedProgramCourses = new List<ProgramCourse>();

        foreach (var pc in program.ProgramCourses)
        {
            if (completedCourseIds.Contains(pc.CourseId))
            {
                completedProgramCourses.Add(pc);
            }
            else if (inProgressCourseIds.Contains(pc.CourseId))
            {
                inProgressProgramCourses.Add(pc);
            }
            else if (pendingCourseIds.Contains(pc.CourseId))
            {
                pendingProgramCourses.Add(pc);
            }
            else if (failedCourseIds.Contains(pc.CourseId))
            {
                failedProgramCourses.Add(pc);
            }
            else
            {
                // Check if available (prerequisites met and year level sufficient)
                var prerequisitesMet = true;
                var yearLevelMet = true;

                // Check prerequisites
                if (pc.Course.PrerequisiteRelations.Any())
                {
                    var requiredPrereqIds = pc.Course.PrerequisiteRelations
                        .Select(pr => pr.PrerequisiteCourseId)
                        .ToList();

                    prerequisitesMet = requiredPrereqIds.All(id => completedCourseIds.Contains(id));
                }

                // Check year level
                if (!string.IsNullOrEmpty(pc.Course.MinimumYearLevel))
                {
                    yearLevelMet = IsYearLevelSufficient(user.YearOfStudy, pc.Course.MinimumYearLevel);
                }

                if (prerequisitesMet && yearLevelMet)
                {
                    availableProgramCourses.Add(pc);
                }
                else
                {
                    lockedProgramCourses.Add(pc);
                }
            }
        }

        // Create enrollment lookup for displaying status
        var enrollmentLookup = allEnrollments.ToDictionary(e => e.CourseId, e => e);

        ViewBag.Program = program;
        ViewBag.TotalCreditsEarned = totalCreditsEarned;
        ViewBag.CompletedCourses = completedProgramCourses;
        ViewBag.InProgressCourses = inProgressProgramCourses;
        ViewBag.PendingCourses = pendingProgramCourses;
        ViewBag.FailedCourses = failedProgramCourses;
        ViewBag.AvailableCourses = availableProgramCourses;
        ViewBag.LockedCourses = lockedProgramCourses;
        ViewBag.StudentYearLevel = user.YearOfStudy;
        ViewBag.EnrollmentLookup = enrollmentLookup;

        return View();
    }

    private bool IsYearLevelSufficient(string? studentYearLevel, string requiredYearLevel)
    {
        if (string.IsNullOrEmpty(studentYearLevel))
        {
            return false;
        }

        var yearLevelHierarchy = new Dictionary<string, int>
        {
            { "Freshman", 1 },
            { "Sophomore", 2 },
            { "Junior", 3 },
            { "Senior", 4 },
            { "Graduate", 5 }
        };

        if (!yearLevelHierarchy.TryGetValue(studentYearLevel, out var studentLevel))
        {
            return false;
        }

        if (!yearLevelHierarchy.TryGetValue(requiredYearLevel, out var requiredLevel))
        {
            return false;
        }

        return studentLevel >= requiredLevel;
    }
}
