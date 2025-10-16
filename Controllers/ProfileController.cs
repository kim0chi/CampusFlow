using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // GET: Profile
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Load enrollments
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == user.Id)
            .OrderByDescending(e => e.SubmittedDate)
            .ToListAsync();

        // Separate enrollments by status
        var completedEnrollments = enrollments.Where(e => e.Status == EnrollmentStatus.Completed).OrderByDescending(e => e.CompletedDate).ToList();
        var inProgressEnrollments = enrollments.Where(e => e.Status == EnrollmentStatus.Enrolled).ToList();
        var failedEnrollments = enrollments.Where(e => e.Status == EnrollmentStatus.Failed).OrderByDescending(e => e.CompletedDate).ToList();

        // Calculate total credits earned from completed courses only
        var totalCreditsEarned = completedEnrollments.Sum(e => e.Course.Credits);

        ViewBag.Enrollments = enrollments;
        ViewBag.CompletedEnrollments = completedEnrollments;
        ViewBag.InProgressEnrollments = inProgressEnrollments;
        ViewBag.FailedEnrollments = failedEnrollments;
        ViewBag.TotalCreditsEarned = totalCreditsEarned;

        return View(user);
    }

    // GET: Profile/Edit
    public async Task<IActionResult> Edit(bool fromRedirect = false)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Show message if redirected because profile is incomplete
        if (fromRedirect && !user.IsProfileCompleted)
        {
            TempData["Info"] = "Welcome! Please complete your profile information before proceeding.";
        }

        return View(user);
    }

    // POST: Profile/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ApplicationUser model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Update user properties
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        user.DateOfBirth = model.DateOfBirth;
        user.Address = model.Address;
        user.City = model.City;
        user.Province = model.Province;
        user.Municipality = model.Municipality;
        user.Barangay = model.Barangay;
        user.PostalCode = model.PostalCode;
        user.Bio = model.Bio;
        user.EmergencyContactName = model.EmergencyContactName;
        user.EmergencyContactPhone = model.EmergencyContactPhone;
        user.EmergencyContactRelationship = model.EmergencyContactRelationship;

        // Only allow Admin/Dean to update academic information
        // Or allow students to update Major if profile is not completed
        if (User.IsInRole("Admin") || User.IsInRole("Dean"))
        {
            user.StudentNumber = model.StudentNumber;
            user.Department = model.Department;
            user.YearOfStudy = model.YearOfStudy;
            user.GPA = model.GPA;
            user.Major = model.Major;
            user.Minor = model.Minor;
        }
        else if (User.IsInRole("Student") && !user.IsProfileCompleted)
        {
            // Allow students to set their Major when completing profile
            user.Major = model.Major;
        }

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            // Check if all required fields are now filled
            if (!user.IsProfileCompleted && user.HasRequiredProfileFields())
            {
                user.IsProfileCompleted = true;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "Profile completed successfully! You can now access all features.";
            }
            else
            {
                TempData["Success"] = "Profile updated successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // GET: Profile/ChangePassword
    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST: Profile/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError(string.Empty, "New password and confirmation password do not match.");
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View();
    }

    // POST: Profile/UploadPicture
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPicture(IFormFile? profilePicture)
    {
        if (profilePicture != null && profilePicture.Length > 0)
        {
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] = "Invalid file type. Please upload a JPG, PNG, or GIF image.";
                return RedirectToAction(nameof(Edit));
            }

            // Validate file size (max 5MB)
            if (profilePicture.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "File size must be less than 5MB.";
                return RedirectToAction(nameof(Edit));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileName = $"{user.Id}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            // Delete old profile picture if exists
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePictureUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Update user profile picture URL
            user.ProfilePictureUrl = $"/uploads/profiles/{fileName}";
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profile picture updated successfully!";
        }
        else
        {
            TempData["Error"] = "Please select a file to upload.";
        }

        return RedirectToAction(nameof(Edit));
    }

    // POST: Profile/RemovePicture
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePicture()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Delete profile picture file if exists
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePictureUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            user.ProfilePictureUrl = null;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profile picture removed successfully!";
        }

        return RedirectToAction(nameof(Edit));
    }
}
