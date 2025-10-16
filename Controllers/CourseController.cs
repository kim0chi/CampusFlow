using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Controllers;

[Authorize(Roles = "Admin")]
public class CourseController : Controller
{
    private readonly ApplicationDbContext _context;

    public CourseController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();

        return View(courses);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Browse()
    {
        var courses = await _context.Courses
            .Where(c => c.IsActive)
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.AvailableCourses = await _context.Courses
            .Where(c => c.IsActive)
            .OrderBy(c => c.CourseCode)
            .ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course, int[] prerequisiteIds)
    {
        if (ModelState.IsValid)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Add prerequisites
            if (prerequisiteIds != null && prerequisiteIds.Length > 0)
            {
                foreach (var prereqId in prerequisiteIds)
                {
                    _context.CoursePrerequisites.Add(new CoursePrerequisite
                    {
                        CourseId = course.Id,
                        PrerequisiteCourseId = prereqId
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Course created successfully.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.AvailableCourses = await _context.Courses
            .Where(c => c.IsActive)
            .OrderBy(c => c.CourseCode)
            .ToListAsync();
        return View(course);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.PrerequisiteRelations)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        ViewBag.AvailableCourses = await _context.Courses
            .Where(c => c.IsActive && c.Id != id)
            .OrderBy(c => c.CourseCode)
            .ToListAsync();

        ViewBag.SelectedPrerequisites = course.PrerequisiteRelations
            .Select(p => p.PrerequisiteCourseId)
            .ToList();

        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course, int[] prerequisiteIds)
    {
        if (id != course.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(course);

                // Remove existing prerequisites
                var existingPrereqs = await _context.CoursePrerequisites
                    .Where(cp => cp.CourseId == id)
                    .ToListAsync();
                _context.CoursePrerequisites.RemoveRange(existingPrereqs);

                // Add new prerequisites
                if (prerequisiteIds != null && prerequisiteIds.Length > 0)
                {
                    foreach (var prereqId in prerequisiteIds)
                    {
                        _context.CoursePrerequisites.Add(new CoursePrerequisite
                        {
                            CourseId = course.Id,
                            PrerequisiteCourseId = prereqId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Course updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.AvailableCourses = await _context.Courses
            .Where(c => c.IsActive && c.Id != id)
            .OrderBy(c => c.CourseCode)
            .ToListAsync();

        ViewBag.SelectedPrerequisites = prerequisiteIds?.ToList() ?? new List<int>();

        return View(course);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .Include(c => c.PrerequisiteRelations)
            .ThenInclude(pr => pr.PrerequisiteCourse)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            course.IsActive = false;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course deactivated successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id);
    }
}
