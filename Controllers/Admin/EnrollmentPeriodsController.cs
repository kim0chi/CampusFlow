using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/EnrollmentPeriods")]
    public class EnrollmentPeriodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentPeriodsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/EnrollmentPeriods
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var periods = await _context.EnrollmentPeriods
                .Include(ep => ep.CreatedByUser)
                .OrderByDescending(ep => ep.CreatedDate)
                .ToListAsync();

            return View(periods);
        }

        // GET: Admin/EnrollmentPeriods/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/EnrollmentPeriods/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrollmentPeriod model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.CloseDate <= model.OpenDate)
            {
                ModelState.AddModelError("CloseDate", "Close date must be after open date.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.CreatedBy = user?.Id;
            model.CreatedDate = DateTime.UtcNow;

            _context.EnrollmentPeriods.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Enrollment period created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/EnrollmentPeriods/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var period = await _context.EnrollmentPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            return View(period);
        }

        // POST: Admin/EnrollmentPeriods/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EnrollmentPeriod model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.CloseDate <= model.OpenDate)
            {
                ModelState.AddModelError("CloseDate", "Close date must be after open date.");
                return View(model);
            }

            var period = await _context.EnrollmentPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            period.Semester = model.Semester;
            period.AcademicYear = model.AcademicYear;
            period.OpenDate = model.OpenDate;
            period.CloseDate = model.CloseDate;
            period.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Enrollment period updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/EnrollmentPeriods/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var period = await _context.EnrollmentPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            _context.EnrollmentPeriods.Remove(period);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Enrollment period deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/EnrollmentPeriods/ToggleActive/5
        [HttpPost("ToggleActive/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var period = await _context.EnrollmentPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            period.IsActive = !period.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Enrollment period {(period.IsActive ? "activated" : "deactivated")} successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
