using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.ServicesForAdmin
{
    [Authorize(Roles = "Manager,Admin")]
    public class EditModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public EditModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
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

            var spring2023_group1_services =  await _context.Spring2023_Group1_Services.FirstOrDefaultAsync(m => m.ServiceID == id);
            if (spring2023_group1_services == null)
            {
                return NotFound();
            }
            Spring2023_Group1_Services = spring2023_group1_services;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Spring2023_Group1_Services).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Spring2023_Group1_ServicesExists(Spring2023_Group1_Services.ServiceID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool Spring2023_Group1_ServicesExists(string id)
        {
          return (_context.Spring2023_Group1_Services?.Any(e => e.ServiceID == id)).GetValueOrDefault();
        }
    }
}
