using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/QuarterPeriods")]
    public class QuarterPeriodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IQuarterPaymentService _quarterPaymentService;

        public QuarterPeriodsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IQuarterPaymentService quarterPaymentService)
        {
            _context = context;
            _userManager = userManager;
            _quarterPaymentService = quarterPaymentService;
        }

        // GET: Admin/QuarterPeriods
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var periods = await _context.QuarterPeriods
                .Include(qp => qp.CreatedByUser)
                .Include(qp => qp.PaymentRequirements)
                .OrderByDescending(qp => qp.CreatedDate)
                .ToListAsync();

            return View(periods);
        }

        // GET: Admin/QuarterPeriods/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/QuarterPeriods/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuarterPeriod model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
                return View(model);
            }

            if (model.PaymentDeadline < model.StartDate)
            {
                ModelState.AddModelError("PaymentDeadline", "Payment deadline should be on or after the start date.");
                return View(model);
            }

            // Check for duplicate quarter period
            var existingPeriod = await _context.QuarterPeriods
                .FirstOrDefaultAsync(qp => qp.Quarter == model.Quarter
                    && qp.Semester == model.Semester
                    && qp.AcademicYear == model.AcademicYear);

            if (existingPeriod != null)
            {
                ModelState.AddModelError("", "A quarter period with this combination already exists.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.CreatedBy = user?.Id;
            model.CreatedDate = DateTime.UtcNow;

            _context.QuarterPeriods.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Quarter period created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/QuarterPeriods/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var period = await _context.QuarterPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            return View(period);
        }

        // POST: Admin/QuarterPeriods/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuarterPeriod model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
                return View(model);
            }

            if (model.PaymentDeadline < model.StartDate)
            {
                ModelState.AddModelError("PaymentDeadline", "Payment deadline should be on or after the start date.");
                return View(model);
            }

            var period = await _context.QuarterPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            period.Quarter = model.Quarter;
            period.Semester = model.Semester;
            period.AcademicYear = model.AcademicYear;
            period.StartDate = model.StartDate;
            period.EndDate = model.EndDate;
            period.PaymentDeadline = model.PaymentDeadline;
            period.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Quarter period updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/QuarterPeriods/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var period = await _context.QuarterPeriods
                .Include(qp => qp.PaymentRequirements)
                .FirstOrDefaultAsync(qp => qp.Id == id);

            if (period == null)
            {
                return NotFound();
            }

            if (period.PaymentRequirements.Any())
            {
                TempData["Error"] = "Cannot delete quarter period with existing payment requirements.";
                return RedirectToAction(nameof(Index));
            }

            _context.QuarterPeriods.Remove(period);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Quarter period deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/QuarterPeriods/ToggleActive/5
        [HttpPost("ToggleActive/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var period = await _context.QuarterPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            period.IsActive = !period.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Quarter period {(period.IsActive ? "activated" : "deactivated")} successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/QuarterPeriods/GenerateRequirements/5
        [HttpPost("GenerateRequirements/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRequirements(int id)
        {
            var period = await _context.QuarterPeriods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            try
            {
                await _quarterPaymentService.GenerateQuarterRequirementsAsync(id);
                TempData["Success"] = "Quarter payment requirements generated successfully for all students with balances.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating requirements: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/QuarterPeriods/Requirements/5
        [HttpGet("Requirements/{id}")]
        public async Task<IActionResult> Requirements(int id)
        {
            var period = await _context.QuarterPeriods
                .Include(qp => qp.PaymentRequirements)
                .ThenInclude(qpr => qpr.Student)
                .FirstOrDefaultAsync(qp => qp.Id == id);

            if (period == null)
            {
                return NotFound();
            }

            return View(period);
        }
    }
}
