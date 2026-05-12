using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.ServicesForAdmin
{
    [Authorize(Roles = "Manager,Admin")]
    public class DeleteModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DeleteModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
      public Spring2023_Group1_Services Spring2023_Group1_Services { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Spring2023_Group1_Services == null)
            {
                return NotFound();
            }

            var spring2023_group1_services = await _context.Spring2023_Group1_Services.FirstOrDefaultAsync(m => m.ServiceID == id);

            if (spring2023_group1_services == null)
            {
                return NotFound();
            }
            else 
            {
                Spring2023_Group1_Services = spring2023_group1_services;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.Spring2023_Group1_Services == null)
            {
                return NotFound();
            }
            var spring2023_group1_services = await _context.Spring2023_Group1_Services.FindAsync(id);

            if (spring2023_group1_services != null)
            {
                Spring2023_Group1_Services = spring2023_group1_services;
                _context.Spring2023_Group1_Services.Remove(Spring2023_Group1_Services);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
