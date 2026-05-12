using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    [Authorize]
    public class ApplicationStatusModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicationStatusModel(
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Spring2023_Group1_Profile_Sys StudentProfile { get; set; }
        public Spring2026_Group1_EmployeeApplication FacultyApplication { get; set; }
        public string ApplicationType { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Check for student profile
            StudentProfile = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(p => p.Email == user.Email);

            if (StudentProfile != null)
            {
                ApplicationType = "Student";
                return Page();
            }

            // Check for faculty application
            FacultyApplication = await _context.Spring2026_Group1_EmployeeApplication
                .FirstOrDefaultAsync(a => a.Email == user.Email);

            if (FacultyApplication != null)
            {
                ApplicationType = "Faculty";
                return Page();
            }

            // No application found
            return RedirectToPage("/Registration/RoleSelection");
        }
    }
}