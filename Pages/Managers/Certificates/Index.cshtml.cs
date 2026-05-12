using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Manager.Certificates
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2026_Group1_Certificate> Certificates { get; set; } = new();
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(string? search)
        {
            SearchTerm = search;

            var query = _context.Spring2026_Group1_Certificate!.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lower = search.ToLower();
                query = query.Where(c =>
                    c.StudentEmail.ToLower().Contains(lower) ||
                    c.StudentName.ToLower().Contains(lower) ||
                    c.ServiceName.ToLower().Contains(lower));
            }

            Certificates = await query
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }
    }
}