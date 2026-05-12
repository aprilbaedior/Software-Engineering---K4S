using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class FacultyApplicationsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FacultyApplicationsModel(
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<Spring2026_Group1_EmployeeApplication> Applications { get; set; } = new();
        public string ActiveTab { get; set; } = "Pending";
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int DeniedCount { get; set; }

        public async Task OnGetAsync(string tab = "Pending")
        {
            ActiveTab = tab;
            
            // Admins only see Manager and Administrator applications
            Applications = await _context.Spring2026_Group1_EmployeeApplication
                .Where(a => a.ApplicationStatus == tab && 
                           (a.DesiredPosition == "Manager" || a.DesiredPosition == "Administrator"))
                .OrderBy(a => a.ApplicationDate)
                .ToListAsync();

            var all = await _context.Spring2026_Group1_EmployeeApplication
                .Where(a => a.DesiredPosition == "Manager" || a.DesiredPosition == "Administrator")
                .ToListAsync();
            
            PendingCount = all.Count(a => a.ApplicationStatus == "Pending");
            ApprovedCount = all.Count(a => a.ApplicationStatus == "Approved");
            DeniedCount = all.Count(a => a.ApplicationStatus == "Denied");
        }

        public async Task<IActionResult> OnPostApproveAsync(int id, string notes)
        {
            var application = await _context.Spring2026_Group1_EmployeeApplication.FindAsync(id);
            
            // Verify this is a Manager or Administrator application
            if (application == null || 
                (application.DesiredPosition != "Manager" && application.DesiredPosition != "Administrator"))
            {
                TempData["ErrorMessage"] = "You can only approve Manager and Administrator applications.";
                return RedirectToPage(new { tab = "Pending" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            
            application.ApplicationStatus = "Approved";
            application.ReviewedBy = currentUser?.Email;
            application.ReviewDate = DateTime.UtcNow;
            application.ReviewNotes = notes;

            await _context.SaveChangesAsync();

            // Create Manager record if applicable
            if (application.DesiredPosition == "Manager")
            {
                await CreateManagerRecordAsync(application);
                await AssignRoleAsync(application.Email, "Manager");
            }
            else if (application.DesiredPosition == "Administrator")
            {
                await AssignRoleAsync(application.Email, "Admin");
            }

            TempData["SuccessMessage"] = $"{application.FullName}'s {application.DesiredPosition.ToLower()} application has been approved and {application.DesiredPosition} role assigned.";
            return RedirectToPage(new { tab = "Pending" });
        }

        public async Task<IActionResult> OnPostDenyAsync(int id, string notes)
        {
            var application = await _context.Spring2026_Group1_EmployeeApplication.FindAsync(id);
            
            // Verify this is a Manager or Administrator application
            if (application == null || 
                (application.DesiredPosition != "Manager" && application.DesiredPosition != "Administrator"))
            {
                TempData["ErrorMessage"] = "You can only deny Manager and Administrator applications.";
                return RedirectToPage(new { tab = "Pending" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            
            application.ApplicationStatus = "Denied";
            application.ReviewedBy = currentUser?.Email;
            application.ReviewDate = DateTime.UtcNow;
            application.ReviewNotes = notes;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{application.FullName}'s {application.DesiredPosition.ToLower()} application has been denied.";
            return RedirectToPage(new { tab = "Pending" });
        }

        private async Task CreateManagerRecordAsync(Spring2026_Group1_EmployeeApplication application)
        {
            var existingManager = await _context.Spring2026_Group1_Manager
                .FirstOrDefaultAsync(m => m.Email == application.Email);

            if (existingManager == null)
            {
                // Generate next ManagerID manually
                int nextManagerId = 1;
                if (await _context.Spring2026_Group1_Manager.AnyAsync())
                {
                    nextManagerId = await _context.Spring2026_Group1_Manager.MaxAsync(m => m.ManagerID) + 1;
                }

                var manager = new Spring2026_Group1_Manager
                {
                    ManagerID = nextManagerId,  // Manually assign ID
                    Email = application.Email,
                    HireDate = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Spring2026_Group1_Manager.Add(manager);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AssignRoleAsync(string email, string roleName)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return;
            }

            // Ensure the role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Check if user already has the role
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}