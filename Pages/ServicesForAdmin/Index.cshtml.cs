using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
namespace CS395SI_Spring2023_Group1.Pages.ServicesForAdmin
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public IList<Spring2023_Group1_Services> Spring2023_Group1_Services { get; set; } = default!;
        public Dictionary<string, int> SectionCounts { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (_context.Spring2023_Group1_Services != null)
            {
                Spring2023_Group1_Services = await _context.Spring2023_Group1_Services.ToListAsync();
            }

            SectionCounts = await _context.Spring2026_Group1_Sections!
                .Where(s => s.ServiceID != null)
                .GroupBy(s => s.ServiceID!)
                .Select(g => new { ServiceID = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ServiceID, x => x.Count);
        }
        
        public IActionResult OnPost()
        {
            // Get the serviceID from the form
            var serviceID = Request.Form["serviceID"].ToString();
            
            // Redirect with the serviceID (it will have the proper char(16) padding from the database)
            return RedirectToPage("/Sections/Index", new { serviceID });
        }

    }
}
