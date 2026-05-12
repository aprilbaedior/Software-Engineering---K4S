using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS395SI_Spring2023_K4S.Model;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_Group1.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages_Strikes
{
    [Authorize(Roles = "Manager,Admin,Instructor,Student")]

    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2026_Group1_Strike> Strikes { get; set; } = new();
        public string ActiveTab { get; set; } = "Pending";
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int DeniedCount { get; set; }

        public async Task OnGetAsync(string tab = "Pending")
        {
            ActiveTab = tab;

            var email = User.Identity?.Name ?? string.Empty;

            IQueryable<Spring2026_Group1_Strike> query = _context.Spring2026_Group1_Strike!
                .Where(s => s.ReviewStatus == tab);

            if (User.IsInRole("Student"))
            {
                // Students see strikes filed against them
                query = query.Where(s => s.StudentEmail == email);
            }
            else if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            {
                // Instructors see strikes they filed
                query = query.Where(s => s.FiledBy == email);
            }
            // Managers/Admins see all strikes

            Strikes = await query
                .OrderByDescending(s => s.FiledDate)
                .ToListAsync();

            IQueryable<Spring2026_Group1_Strike> allQuery = _context.Spring2026_Group1_Strike!;
            if (User.IsInRole("Student"))
            {
                allQuery = allQuery.Where(s => s.StudentEmail == email);
            }
            else if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            {
                allQuery = allQuery.Where(s => s.FiledBy == email);
            }

            var all = await allQuery.ToListAsync();
            PendingCount = all.Count(s => s.ReviewStatus == "Pending");
            ApprovedCount = all.Count(s => s.ReviewStatus == "Approved");
            DeniedCount = all.Count(s => s.ReviewStatus == "Denied");
        }

        public async Task<IActionResult> OnPostAsync(int strikeId, string action, string? managerNotes)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var strike = await _context.Spring2026_Group1_Strike!.FindAsync(strikeId);
            if (strike == null)
            {
                TempData["ErrorMessage"] = "Strike not found.";
                return RedirectToPage(new { tab = "Pending" });
            }

            strike.ReviewStatus = action == "Approve" ? "Approved" : "Denied";
            strike.ReviewedBy = User.Identity?.Name ?? "Unknown";
            strike.ReviewDate = DateTime.UtcNow;
            strike.ManagerNotes = managerNotes;

            await _context.SaveChangesAsync();

            var result = action == "Approve" ? "approved" : "denied";
            TempData["SuccessMessage"] = $"Strike #{strikeId} has been {result}.";
            return RedirectToPage(new { tab = "Pending" });
        }
    }
}