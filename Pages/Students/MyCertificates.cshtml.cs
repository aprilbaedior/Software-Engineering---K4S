using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Students
{
    [Authorize(Roles = "Student")]
    public class MyCertificatesModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public MyCertificatesModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2026_Group1_Certificate> Certificates { get; set; } = new();

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name ?? string.Empty;

            Certificates = await _context.Spring2026_Group1_Certificate!
                .Where(c => c.StudentEmail == email)
                .OrderByDescending(c => c.IssuedDate)
                .ToListAsync();
        }
    }
}
