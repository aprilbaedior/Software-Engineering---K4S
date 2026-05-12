using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    [Authorize]
    public class RoleSelectionModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleSelectionModel(
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public string SelectedRole { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Check if user already has any application
            var hasStudentProfile = await _context.Spring2023_Group1_Profile_Sys
                .AnyAsync(p => p.Email == user.Email);
            var hasFacultyApp = await _context.Spring2026_Group1_EmployeeApplication
                .AnyAsync(a => a.Email == user.Email);

            if (hasStudentProfile || hasFacultyApp)
            {
                return RedirectToPage("/Registration/ApplicationStatus");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(SelectedRole))
            {
                ModelState.AddModelError("", "Please select a role.");
                return Page();
            }

            return SelectedRole switch
            {
                "Student" => RedirectToPage("/Scheduling/Registration/Create"),
                "Faculty" => RedirectToPage("/Registration/FacultyApplication"),
                _ => Page()
            };
        }
    }
}