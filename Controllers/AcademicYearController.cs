using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Controllers;

[Authorize(Roles = "Admin")]
public class AcademicYearController : Controller
{
    private readonly ApplicationDbContext _context;

    public AcademicYearController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: AcademicYear
    public async Task<IActionResult> Index()
    {
        var academicYears = await _context.AcademicYears
            .OrderByDescending(ay => ay.Year)
            .ToListAsync();
        return View(academicYears);
    }

    // GET: AcademicYear/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AcademicYear/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AcademicYear academicYear)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Check if year already exists
                var exists = await _context.AcademicYears
                    .AnyAsync(ay => ay.Year == academicYear.Year);

                if (exists)
                {
                    ModelState.AddModelError("Year", "This academic year already exists.");
                    return View(academicYear);
                }

                // If this is set as current year, unset all others
                if (academicYear.IsCurrentYear)
                {
                    var currentYears = await _context.AcademicYears
                        .Where(ay => ay.IsCurrentYear)
                        .ToListAsync();

                    foreach (var year in currentYears)
                    {
                        year.IsCurrentYear = false;
                    }
                }

                academicYear.CreatedDate = DateTime.UtcNow;
                _context.Add(academicYear);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Academic year {academicYear.Year} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating academic year: {ex.Message}");
            }
        }
        return View(academicYear);
    }

    // GET: AcademicYear/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var academicYear = await _context.AcademicYears.FindAsync(id);
        if (academicYear == null)
        {
            return NotFound();
        }
        return View(academicYear);
    }

    // POST: AcademicYear/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AcademicYear academicYear)
    {
        if (id != academicYear.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Check if year already exists (excluding current record)
                var exists = await _context.AcademicYears
                    .AnyAsync(ay => ay.Year == academicYear.Year && ay.Id != id);

                if (exists)
                {
                    ModelState.AddModelError("Year", "This academic year already exists.");
                    return View(academicYear);
                }

                // If this is set as current year, unset all others
                if (academicYear.IsCurrentYear)
                {
                    var currentYears = await _context.AcademicYears
                        .Where(ay => ay.IsCurrentYear && ay.Id != id)
                        .ToListAsync();

                    foreach (var year in currentYears)
                    {
                        year.IsCurrentYear = false;
                    }
                }

                _context.Update(academicYear);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Academic year {academicYear.Year} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcademicYearExists(academicYear.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating academic year: {ex.Message}");
            }
        }
        return View(academicYear);
    }

    // POST: AcademicYear/SetCurrent/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCurrent(int id)
    {
        try
        {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }

            // Unset all other current years
            var currentYears = await _context.AcademicYears
                .Where(ay => ay.IsCurrentYear)
                .ToListAsync();

            foreach (var year in currentYears)
            {
                year.IsCurrentYear = false;
            }

            // Set this as current
            academicYear.IsCurrentYear = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Academic year {academicYear.Year} is now set as current!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error setting current year: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: AcademicYear/ToggleActive/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        try
        {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }

            academicYear.IsActive = !academicYear.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Academic year {academicYear.Year} is now {(academicYear.IsActive ? "active" : "inactive")}!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error toggling active status: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: AcademicYear/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }

            // Check if any enrollment batches use this academic year
            var hasEnrollments = await _context.EnrollmentBatches
                .AnyAsync(eb => eb.AcademicYear == academicYear.Year);

            if (hasEnrollments)
            {
                TempData["Error"] = $"Cannot delete academic year {academicYear.Year} because it has associated enrollments.";
                return RedirectToAction(nameof(Index));
            }

            _context.AcademicYears.Remove(academicYear);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Academic year {academicYear.Year} deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting academic year: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AcademicYearExists(int id)
    {
        return _context.AcademicYears.Any(e => e.Id == id);
    }
}
