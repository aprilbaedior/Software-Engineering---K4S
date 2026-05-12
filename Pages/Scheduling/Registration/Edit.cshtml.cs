using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    public class EditModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public EditModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2023_Group1_Profile_Sys Spring2023_Group1_Profile_Sys { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Spring2023_Group1_Profile_Sys == null)
            {
                return NotFound();
            }

            var spring2023_group1_profile_sys =  await _context.Spring2023_Group1_Profile_Sys.FirstOrDefaultAsync(m => m.Email == id);
            if (spring2023_group1_profile_sys == null)
            {
                return NotFound();
            }
            Spring2023_Group1_Profile_Sys = spring2023_group1_profile_sys;
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

            _context.Attach(Spring2023_Group1_Profile_Sys).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Spring2023_Group1_Profile_SysExists(Spring2023_Group1_Profile_Sys.Email))
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

        private bool Spring2023_Group1_Profile_SysExists(string id)
        {
          return _context.Spring2023_Group1_Profile_Sys.Any(e => e.Email == id);
        }
    }
}
