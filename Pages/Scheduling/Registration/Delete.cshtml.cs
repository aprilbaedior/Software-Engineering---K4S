using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    public class DeleteModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DeleteModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
      public Spring2023_Group1_Profile_Sys Spring2023_Group1_Profile_Sys { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Spring2023_Group1_Profile_Sys == null)
            {
                return NotFound();
            }

            var spring2023_group1_profile_sys = await _context.Spring2023_Group1_Profile_Sys.FirstOrDefaultAsync(m => m.Email == id);

            if (spring2023_group1_profile_sys == null)
            {
                return NotFound();
            }
            else 
            {
                Spring2023_Group1_Profile_Sys = spring2023_group1_profile_sys;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.Spring2023_Group1_Profile_Sys == null)
            {
                return NotFound();
            }
            var spring2023_group1_profile_sys = await _context.Spring2023_Group1_Profile_Sys.FindAsync(id);

            if (spring2023_group1_profile_sys != null)
            {
                Spring2023_Group1_Profile_Sys = spring2023_group1_profile_sys;
                _context.Spring2023_Group1_Profile_Sys.Remove(Spring2023_Group1_Profile_Sys);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
