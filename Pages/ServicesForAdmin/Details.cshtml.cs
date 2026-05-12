using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.ServicesForAdmin
{
    [Authorize(Roles = "Manager,Admin")]
    public class DetailsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public DetailsModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public Spring2023_Group1_Services Spring2023_Group1_Services { get; set; } = default!;
        public List<Spring2026_Group1_Sections> Sections { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Spring2023_Group1_Services == null)
            {
                return NotFound();
            }

            var service = await _context.Spring2023_Group1_Services
                .FirstOrDefaultAsync(m => m.ServiceID == id);

            if (service == null)
            {
                return NotFound();
            }

            Spring2023_Group1_Services = service;

            // Load sections for this service
            Sections = await _context.Spring2026_Group1_Sections!
                .Where(s => s.ServiceID == id)
                .OrderBy(s => s.Status)
                .ThenBy(s => s.SectionID)
                .ToListAsync();

            return Page();
        }
    }
}
