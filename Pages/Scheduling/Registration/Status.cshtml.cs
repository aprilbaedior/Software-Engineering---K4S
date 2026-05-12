using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    [Authorize]
    public class StatusModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StatusModel(CS395SI_Spring2023_Group1Context context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Spring2023_Group1_Profile_Sys? StudentProfile { get; set; }
        public Spring2026_Group1_EmployeeApplication? EmployeeApplication { get; set; }
        public bool HasAnyApplication { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ErrorMessage = "Unable to load user information.";
                return;
            }

            var email = user.Email;

            // Check for Student application
            StudentProfile = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(p => p.Email == email);

            // Check for Employee application (Instructor/Manager/Admin)
            EmployeeApplication = await _context.Spring2026_Group1_EmployeeApplication
                .OrderByDescending(a => a.ApplicationDate)
                .FirstOrDefaultAsync(a => a.Email == email);

            HasAnyApplication = StudentProfile != null || EmployeeApplication != null;
        }
    }
}