using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Filters;

public class RequireProfileCompletionFilter : IAsyncActionFilter
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RequireProfileCompletionFilter(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        // Skip if user is not authenticated
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            await next();
            return;
        }

        // Get the current user from database
        var currentUser = await _userManager.GetUserAsync(user);
        if (currentUser == null)
        {
            await next();
            return;
        }

        // Only enforce for Students
        var roles = await _userManager.GetRolesAsync(currentUser);
        if (!roles.Contains("Student"))
        {
            await next();
            return;
        }

        // Allow access to Profile, Account, and Home controllers
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        if (controllerName == "Profile" || controllerName == "Account" || controllerName == "Home")
        {
            await next();
            return;
        }

        // Check approval status first
        if (currentUser.ApprovalStatus == "Pending")
        {
            context.HttpContext.Response.Redirect("/Account/PendingApproval");
            return;
        }

        if (currentUser.ApprovalStatus == "Rejected")
        {
            context.HttpContext.Response.Redirect("/Account/Rejected");
            return;
        }

        // Check if profile is completed
        if (!currentUser.IsProfileCompleted && currentUser.ApprovalStatus == "Approved")
        {
            // Redirect to profile edit page with a message
            context.HttpContext.Response.Redirect("/Profile/Edit?fromRedirect=true");
            return;
        }

        await next();
    }
}
