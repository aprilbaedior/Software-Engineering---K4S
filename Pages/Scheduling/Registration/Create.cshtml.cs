using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    public class CreateModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Check if user already has a profile using email address
            var existingProfile = _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefault(p => p.Email == user.Email);
            
            if (existingProfile != null)
            {
                // User already has an application, redirect to status page
                return RedirectToPage("./Status", new { id = existingProfile.ID });
            }
            
            // Pre-populate the email from the user's account
            Spring2023_Group1_Profile_Sys = new Spring2023_Group1_Profile_Sys
            {
                Email = user.Email
            };
            
            return Page();
        }

        [BindProperty]
        public Spring2023_Group1_Profile_Sys Spring2023_Group1_Profile_Sys { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    // Ensure the email matches the current user's email
    var user = await _userManager.GetUserAsync(User);
    if (user != null)
    {
        Spring2023_Group1_Profile_Sys.Email = user.Email;
    }
    
    // Clear the password field for security
    Spring2023_Group1_Profile_Sys.Password = null;
    
    // Set default application status
    Spring2023_Group1_Profile_Sys.ApplicationStatus = "Pending";

    _context.Spring2023_Group1_Profile_Sys.Add(Spring2023_Group1_Profile_Sys);
    await _context.SaveChangesAsync();
    
    if (User.IsInRole("Admin"))
    {
        return RedirectToPage("/Scheduling/Registration/Index");
    }
    else
    {
        return RedirectToPage("./Status");
    }
}
    }
}