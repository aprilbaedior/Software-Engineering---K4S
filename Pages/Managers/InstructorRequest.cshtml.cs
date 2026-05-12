using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Manager
{
    [Authorize(Roles = "Manager,Admin")]
    public class InstructorRequestsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InstructorRequestsModel(CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Spring2026_Group1_InstructorRequest> Requests { get; set; } = new();
        public string ActiveTab { get; set; } = "Pending";
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int DeniedCount { get; set; }

        public async Task OnGetAsync(string tab = "Pending")
        {
            ActiveTab = tab;
            Requests = await _context.Spring2026_Group1_InstructorRequest!
                .Where(r => r.Status == tab)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            var all = await _context.Spring2026_Group1_InstructorRequest!.ToListAsync();
            PendingCount = all.Count(r => r.Status == "Pending");
            ApprovedCount = all.Count(r => r.Status == "Approved");
            DeniedCount = all.Count(r => r.Status == "Denied");
        }

        public async Task<IActionResult> OnPostApproveAsync(int requestId)
        {
            var request = await _context.Spring2026_Group1_InstructorRequest!.FindAsync(requestId);
            if (request == null)
            {
                TempData["ErrorMessage"] = "Request not found.";
                return RedirectToPage(new { tab = "Pending" });
            }

            var managerEmail = User.Identity?.Name ?? "Unknown";

            // Update request status
            request.Status = "Approved";
            request.ReviewedBy = managerEmail;
            request.ReviewDate = DateTime.UtcNow;

            // Create Instructor record
            var instructor = new Spring2026_Group1_Instructor
            {
                Email = request.Email,  // ✅ FIXED
                FullName = request.FullName,
                Speciality = request.Speciality,
                HireDate = DateTime.UtcNow,
                IsActive = true,
                ApprovedDate = DateTime.UtcNow,
                ApprovedBy = managerEmail,
                Status = "Active"
            };
            _context.Spring2026_Group1_Instructor!.Add(instructor);

            // Grant Instructor role via Identity
            var user = await _userManager.FindByEmailAsync(request.Email);  // ✅ FIXED
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Instructor"); 
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{request.FullName} has been approved as an Instructor!";
            return RedirectToPage(new { tab = "Pending" });
        }

        public async Task<IActionResult> OnPostDenyAsync(int requestId, string denialReason)
        {
            var request = await _context.Spring2026_Group1_InstructorRequest!.FindAsync(requestId);
            if (request == null)
            {
                TempData["ErrorMessage"] = "Request not found.";
                return RedirectToPage(new { tab = "Pending" });
            }

            request.Status = "Denied";
            request.ReviewedBy = User.Identity?.Name ?? "Unknown";
            request.ReviewDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{request.FullName}'s instructor request has been denied.";
            return RedirectToPage(new { tab = "Pending" });
        }
    }
}