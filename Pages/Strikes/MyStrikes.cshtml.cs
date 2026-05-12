using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS395SI_Spring2023_K4S.Model;
using CS395SI_Spring2023_Group1.Data;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Strikes
{
    [Authorize]
    public class MyStrikesModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public MyStrikesModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2026_Group1_Strike> ApprovedStrikes { get; set; } = new();
        public List<Spring2026_Group1_Strike> PendingStrikes { get; set; } = new();
        public List<Spring2026_Group1_Strike> DeniedStrikes { get; set; } = new();
        public int TotalApprovedCount { get; set; }

        public async Task OnGetAsync()
        {
            var studentEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(studentEmail))
            {
                return;
            }

            var allStrikes = await _context.Spring2026_Group1_Strike!
                .Where(s => s.StudentEmail == studentEmail)
                .OrderByDescending(s => s.FiledDate)
                .ToListAsync();

            ApprovedStrikes = allStrikes.Where(s => s.ReviewStatus == "Approved").ToList();
            PendingStrikes = allStrikes.Where(s => s.ReviewStatus == "Pending").ToList();
            DeniedStrikes = allStrikes.Where(s => s.ReviewStatus == "Denied").ToList();

            TotalApprovedCount = ApprovedStrikes.Count;
        }
    }
}