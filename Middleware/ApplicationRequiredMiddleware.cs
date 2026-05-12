using CS395SI_Spring2023_Group1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Middleware
{
    public class ApplicationRequiredMiddleware
    {
        private readonly RequestDelegate _next;

        public ApplicationRequiredMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, 
            CS395SI_Spring2023_Group1Context dbContext,
            UserManager<IdentityUser> userManager)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value?.ToLower() ?? "";

            var exemptPaths = new[]
            {
                "/identity/account/logout",
                "/identity/account/accessdenied",
                "/registration/roleselection",
                "/registration/facultyapplication",
                "/registration/applicationstatus",
                "/scheduling/registration/create",
                "/scheduling/registration/status",
                "/scheduling/registration/edit",
                "/privacy",
                "/error"
            };

            if (exemptPaths.Any(p => path.StartsWith(p.ToLower())))
            {
                await _next(context);
                return;
            }

            var user = await userManager.GetUserAsync(context.User);
            if (user != null)
            {
                // Exempt Admin users from application requirement
                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _next(context);
                    return;
                }

                // Check if user has ANY application
                var hasStudentProfile = await dbContext.Spring2023_Group1_Profile_Sys
                    .AnyAsync(p => p.Email == user.Email);
                var hasFacultyApp = await dbContext.Spring2026_Group1_EmployeeApplication
                    .AnyAsync(a => a.Email == user.Email);

                // If no application exists, redirect to role selection
                if (!hasStudentProfile && !hasFacultyApp)
                {
                    context.Response.Redirect("/Registration/RoleSelection");
                    return;
                }
            }

            await _next(context);
        }
    }
}