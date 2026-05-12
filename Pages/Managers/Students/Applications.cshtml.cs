using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using CS395SI_Spring2023_Group1.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Manager.Students
{
    [Authorize(Roles = "Manager,Admin")]
    public class ApplicationsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ApplicationsModel(
            CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public List<Spring2023_Group1_Profile_Sys> Applications { get; set; } = new();
        public string ActiveTab { get; set; } = "Pending";
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int DeniedCount { get; set; }

        public async Task OnGetAsync(string tab = "Pending")
        {
            ActiveTab = tab;
            Applications = await _context.Spring2023_Group1_Profile_Sys
                .Where(s => s.ApplicationStatus == tab)
                .OrderBy(s => s.Name)
                .ToListAsync();

            var all = await _context.Spring2023_Group1_Profile_Sys.ToListAsync();
            PendingCount = all.Count(s => s.ApplicationStatus == "Pending");
            ApprovedCount = all.Count(s => s.ApplicationStatus == "Approved");
            DeniedCount = all.Count(s => s.ApplicationStatus == "Denied");
        }

        public async Task<IActionResult> OnPostApproveAsync(string email)
        {
            var profile = await _context.Spring2023_Group1_Profile_Sys.FindAsync(email);
            if (profile == null)
            {
                TempData["ErrorMessage"] = "Application not found.";
                return RedirectToPage(new { tab = "Pending" });
            }

            profile.ApplicationStatus = "Approved";

            // Assign Student role with error checking
            var roleAssigned = await AssignStudentRoleAsync(email);

            if (roleAssigned)
            {
                var existingStudent = await _context.Spring2026_Group1_Students
                    .FirstOrDefaultAsync(s => s.Email == email);

                if (existingStudent == null)
                {
                    // Generate next StudentID manually
                    int nextStudentId = 1;
                    if (await _context.Spring2026_Group1_Students.AnyAsync())
                    {
                        nextStudentId = await _context.Spring2026_Group1_Students.MaxAsync(s => s.StudentID) + 1;
                    }

                    var student = new Spring2026_Group1_Students
                    {
                        StudentID = nextStudentId,  //Manually assign ID
                        Email = email,
                        EnrollmentDate = DateTime.UtcNow,
                        IsActive = true,
                        EndDate = null
                    };

                    _context.Spring2026_Group1_Students.Add(student);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{profile.Name}'s application has been approved and Student role assigned. The user should sign out and back in to access student features.";
            }
            else
            {
                await _context.SaveChangesAsync();
                TempData["WarningMessage"] = $"{profile.Name}'s application has been approved, but there was an issue assigning the Student role. Please verify the user account exists.";
            }

            return RedirectToPage(new { tab = "Pending" });
        }

        public async Task<IActionResult> OnPostDenyAsync(string email)
        {
            var profile = await _context.Spring2023_Group1_Profile_Sys.FindAsync(email);
            if (profile != null)
            {
                profile.ApplicationStatus = "Denied";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{profile.Name}'s application has been denied.";
            }
            return RedirectToPage(new { tab = "Pending" });
        }

        private async Task<bool> AssignStudentRoleAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // User doesn't exist in Identity system
                return false;
            }

            // Use the constant to ensure proper capitalization
            // Ensure the Student role exists
            if (!await _roleManager.RoleExistsAsync(MyConstants.ROLE_STUDENT))
            {
                await _roleManager.CreateAsync(new IdentityRole(MyConstants.ROLE_STUDENT));
            }

            // Check if user already has the role
            if (!await _userManager.IsInRoleAsync(user, MyConstants.ROLE_STUDENT))
            {
                var result = await _userManager.AddToRoleAsync(user, MyConstants.ROLE_STUDENT);
                
                // If successful, refresh the sign-in
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                }
                
                return result.Succeeded;
            }

            // User already has the role
            return true;
        }
    }
}