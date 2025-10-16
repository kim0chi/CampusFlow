using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize(Roles = "Admin,Dean")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IStudentNumberService _studentNumberService;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IStudentNumberService studentNumberService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _studentNumberService = studentNumberService;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> ManageRoles(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        ViewBag.UserRoles = userRoles;
        ViewBag.AllRoles = allRoles;
        ViewBag.User = user;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            TempData["Error"] = "Role does not exist.";
            return RedirectToAction(nameof(ManageRoles), new { id = userId });
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            TempData["Success"] = $"Role '{roleName}' added successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to add role.";
        }

        return RedirectToAction(nameof(ManageRoles), new { id = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            TempData["Success"] = $"Role '{roleName}' removed successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to remove role.";
        }

        return RedirectToAction(nameof(ManageRoles), new { id = userId });
    }

    // Course Management
    public async Task<IActionResult> ManageCourses()
    {
        var courses = await _context.Courses
            .Where(c => c.IsActive)
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> ManageCourseEnrollments(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.GradedBy)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        // Filter to only show enrolled students (actively taking the course)
        var enrolledStudents = course.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Enrolled || e.Status == EnrollmentStatus.Completed || e.Status == EnrollmentStatus.Failed)
            .OrderBy(e => e.Student.LastName)
            .ThenBy(e => e.Student.FirstName)
            .ToList();

        ViewBag.Course = course;
        ViewBag.Enrollments = enrolledStudents;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkGrade(int enrollmentId, string grade, string status)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

        if (enrollment == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        // Update enrollment
        enrollment.Grade = grade;
        enrollment.GradedById = currentUser?.Id;
        enrollment.GradedDate = DateTime.UtcNow;

        if (status == "Completed")
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedDate = DateTime.UtcNow;
        }
        else if (status == "Failed")
        {
            enrollment.Status = EnrollmentStatus.Failed;
            enrollment.CompletedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Grade '{grade}' assigned successfully.";
        return RedirectToAction(nameof(ManageCourseEnrollments), new { id = enrollment.CourseId });
    }

    // Student Academic Management
    public async Task<IActionResult> ManageStudents(string? search, int? departmentId, string? yearLevel)
    {
        var studentsQuery = _userManager.Users.Include(u => u.Department).AsQueryable();

        // Filter to only show users with Student role
        var studentUsers = new List<ApplicationUser>();
        foreach (var user in await studentsQuery.ToListAsync())
        {
            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                studentUsers.Add(user);
            }
        }

        // Apply filters
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            studentUsers = studentUsers.Where(s =>
                (s.FirstName != null && s.FirstName.ToLower().Contains(search)) ||
                (s.LastName != null && s.LastName.ToLower().Contains(search)) ||
                (s.StudentNumber != null && s.StudentNumber.ToLower().Contains(search)) ||
                (s.Email != null && s.Email.ToLower().Contains(search))
            ).ToList();
        }

        if (departmentId.HasValue)
        {
            studentUsers = studentUsers.Where(s => s.DepartmentId == departmentId.Value).ToList();
        }

        if (!string.IsNullOrEmpty(yearLevel))
        {
            studentUsers = studentUsers.Where(s => s.YearOfStudy == yearLevel).ToList();
        }

        // Get distinct departments and year levels for filters
        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        var yearLevels = new[] { "Freshman", "Sophomore", "Junior", "Senior", "Graduate" };

        ViewBag.Departments = departments;
        ViewBag.YearLevels = yearLevels;
        ViewBag.Search = search;
        ViewBag.SelectedDepartmentId = departmentId;
        ViewBag.SelectedYearLevel = yearLevel;

        return View(studentUsers.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList());
    }

    public async Task<IActionResult> EditStudentAcademics(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var student = await _userManager.FindByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        // Verify the user is a student
        if (!await _userManager.IsInRoleAsync(student, "Student"))
        {
            TempData["Error"] = "The selected user is not a student.";
            return RedirectToAction(nameof(ManageStudents));
        }

        // Get student's enrollments
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == id)
            .OrderByDescending(e => e.SubmittedDate)
            .ToListAsync();

        ViewBag.Enrollments = enrollments;

        // Get available programs for major selection
        var programs = await _context.Programs
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.Programs = programs;

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStudentAcademics(string id, string studentNumber, int? departmentId,
        string yearOfStudy, string? major, string? minor, decimal? gpa)
    {
        var student = await _userManager.FindByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        // Update academic information
        student.StudentNumber = studentNumber;
        student.DepartmentId = departmentId;
        student.YearOfStudy = yearOfStudy;
        student.Major = major;
        student.Minor = minor;
        student.GPA = gpa;

        var result = await _userManager.UpdateAsync(student);
        if (result.Succeeded)
        {
            TempData["Success"] = $"Academic information updated successfully for {student.FullName}.";
            return RedirectToAction(nameof(ManageStudents));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        // Reload data for view
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == id)
            .OrderByDescending(e => e.SubmittedDate)
            .ToListAsync();

        ViewBag.Enrollments = enrollments;

        var programs = await _context.Programs
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.Programs = programs;

        return View(student);
    }

    // Pending Student Registrations
    public async Task<IActionResult> PendingRegistrations()
    {
        var pendingStudents = new List<ApplicationUser>();

        foreach (var user in await _userManager.Users.ToListAsync())
        {
            if (await _userManager.IsInRoleAsync(user, "Student") && user.ApprovalStatus == "Pending")
            {
                pendingStudents.Add(user);
            }
        }

        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        ViewBag.Departments = departments;

        return View(pendingStudents.OrderBy(s => s.Email).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveStudent(string userId, int departmentId)
    {
        var student = await _userManager.FindByIdAsync(userId);
        if (student == null)
        {
            TempData["Error"] = "Student not found.";
            return RedirectToAction(nameof(PendingRegistrations));
        }

        var currentUser = await _userManager.GetUserAsync(User);

        // Generate student number
        var studentNumber = await _studentNumberService.GenerateStudentNumberAsync(departmentId);

        // Update student
        student.StudentNumber = studentNumber;
        student.DepartmentId = departmentId;
        student.ApprovalStatus = "Approved";
        student.ApprovedById = currentUser?.Id;
        student.ApprovalDate = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(student);
        if (result.Succeeded)
        {
            TempData["Success"] = $"Student {student.FullName} approved successfully. Student Number: {studentNumber}";
        }
        else
        {
            TempData["Error"] = "Failed to approve student.";
        }

        return RedirectToAction(nameof(PendingRegistrations));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectStudent(string userId, string rejectionReason)
    {
        var student = await _userManager.FindByIdAsync(userId);
        if (student == null)
        {
            TempData["Error"] = "Student not found.";
            return RedirectToAction(nameof(PendingRegistrations));
        }

        student.ApprovalStatus = "Rejected";
        student.RejectionReason = rejectionReason;

        var result = await _userManager.UpdateAsync(student);
        if (result.Succeeded)
        {
            TempData["Success"] = $"Student {student.FullName} registration rejected.";
        }
        else
        {
            TempData["Error"] = "Failed to reject student.";
        }

        return RedirectToAction(nameof(PendingRegistrations));
    }

    // Department Management
    public async Task<IActionResult> ManageDepartments()
    {
        var departments = await _context.Departments
            .OrderBy(d => d.Name)
            .ToListAsync();

        return View(departments);
    }

    [HttpGet]
    public IActionResult CreateDepartment()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDepartment(string name, string code, string? description)
    {
        if (await _context.Departments.AnyAsync(d => d.Code == code))
        {
            TempData["Error"] = "A department with this code already exists.";
            return View();
        }

        var department = new Models.Department
        {
            Name = name,
            Code = code,
            Description = description,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Department '{name}' created successfully.";
        return RedirectToAction(nameof(ManageDepartments));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDepartmentStatus(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            TempData["Error"] = "Department not found.";
            return RedirectToAction(nameof(ManageDepartments));
        }

        department.IsActive = !department.IsActive;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Department '{department.Name}' {(department.IsActive ? "activated" : "deactivated")} successfully.";
        return RedirectToAction(nameof(ManageDepartments));
    }

    // Program Management
    public async Task<IActionResult> ManagePrograms()
    {
        var programs = await _context.Programs
            .Include(p => p.Department)
            .OrderBy(p => p.Department!.Name)
            .ThenBy(p => p.Name)
            .ToListAsync();

        return View(programs);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProgram()
    {
        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        ViewBag.Departments = departments;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProgram(string name, int? departmentId, string? description, int totalCreditsRequired)
    {
        if (await _context.Programs.AnyAsync(p => p.Name == name && p.DepartmentId == departmentId))
        {
            TempData["Error"] = "A program with this name already exists in the selected department.";

            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
            ViewBag.Departments = departments;

            return View();
        }

        var program = new Models.Program
        {
            Name = name,
            DepartmentId = departmentId,
            Description = description,
            TotalCreditsRequired = totalCreditsRequired,
            IsActive = true
        };

        _context.Programs.Add(program);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Program '{name}' created successfully.";
        return RedirectToAction(nameof(ManagePrograms));
    }

    [HttpGet]
    public async Task<IActionResult> EditProgram(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var program = await _context.Programs
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (program == null)
        {
            return NotFound();
        }

        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        ViewBag.Departments = departments;
        return View(program);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProgram(int id, string name, int? departmentId, string? description, int totalCreditsRequired)
    {
        var program = await _context.Programs.FindAsync(id);
        if (program == null)
        {
            return NotFound();
        }

        program.Name = name;
        program.DepartmentId = departmentId;
        program.Description = description;
        program.TotalCreditsRequired = totalCreditsRequired;

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Program '{name}' updated successfully.";
        return RedirectToAction(nameof(ManagePrograms));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleProgramStatus(int id)
    {
        var program = await _context.Programs.FindAsync(id);
        if (program == null)
        {
            TempData["Error"] = "Program not found.";
            return RedirectToAction(nameof(ManagePrograms));
        }

        program.IsActive = !program.IsActive;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Program '{program.Name}' {(program.IsActive ? "activated" : "deactivated")} successfully.";
        return RedirectToAction(nameof(ManagePrograms));
    }

    public async Task<IActionResult> ManageProgramCourses(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var program = await _context.Programs
            .Include(p => p.Department)
            .Include(p => p.ProgramCourses)
            .ThenInclude(pc => pc.Course)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (program == null)
        {
            return NotFound();
        }

        // Get all active courses not already in the program
        var programCourseIds = program.ProgramCourses.Select(pc => pc.CourseId).ToList();
        var availableCourses = await _context.Courses
            .Where(c => c.IsActive && !programCourseIds.Contains(c.Id))
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();

        ViewBag.Program = program;
        ViewBag.AvailableCourses = availableCourses;

        return View(program);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProgramCourse(int programId, int courseId, bool isRequired, string? yearRecommended, string? semesterRecommended)
    {
        // Check if course already exists in program
        if (await _context.ProgramCourses.AnyAsync(pc => pc.ProgramId == programId && pc.CourseId == courseId))
        {
            TempData["Error"] = "This course is already in the program curriculum.";
            return RedirectToAction(nameof(ManageProgramCourses), new { id = programId });
        }

        var programCourse = new Models.ProgramCourse
        {
            ProgramId = programId,
            CourseId = courseId,
            IsRequired = isRequired,
            YearRecommended = yearRecommended,
            SemesterRecommended = semesterRecommended
        };

        _context.ProgramCourses.Add(programCourse);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Course added to curriculum successfully.";
        return RedirectToAction(nameof(ManageProgramCourses), new { id = programId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProgramCourse(int id, int programId)
    {
        var programCourse = await _context.ProgramCourses.FindAsync(id);
        if (programCourse == null)
        {
            TempData["Error"] = "Program course not found.";
            return RedirectToAction(nameof(ManageProgramCourses), new { id = programId });
        }

        _context.ProgramCourses.Remove(programCourse);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Course removed from curriculum successfully.";
        return RedirectToAction(nameof(ManageProgramCourses), new { id = programId });
    }
}

