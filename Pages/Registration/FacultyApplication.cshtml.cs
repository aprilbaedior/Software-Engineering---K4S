using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    [Authorize]
    public class FacultyApplicationModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FacultyApplicationModel(
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Spring2026_Group1_EmployeeApplication Application { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Check if already has an application
            var existing = await _context.Spring2026_Group1_EmployeeApplication
                .FirstOrDefaultAsync(a => a.Email == user.Email);

            if (existing != null)
            {
                return RedirectToPage("/Registration/ApplicationStatus");
            }

            Application = new Spring2026_Group1_EmployeeApplication
            {
                Email = user.Email
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            Application.Email = user.Email;
            Application.ApplicationStatus = "Pending";
            Application.ApplicationDate = DateTime.UtcNow;

            _context.Spring2026_Group1_EmployeeApplication.Add(Application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your faculty application has been submitted successfully!";
            return RedirectToPage("/Registration/ApplicationStatus");
        }
    }
}