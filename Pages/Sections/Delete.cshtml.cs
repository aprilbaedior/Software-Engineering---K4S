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

namespace CS395SI_Spring2023_Group1.Pages.Sections
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
      public Spring2026_Group1_Sections Spring2026_Group1_Sections { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Spring2026_Group1_Sections == null)
            {
                return NotFound();
            }

            var springSection = await _context.Spring2026_Group1_Sections.FirstOrDefaultAsync(m => m.SectionID == id);

            if (springSection == null)
            {
                return NotFound();
            }
            else 
            {
                Spring2026_Group1_Sections = springSection;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Spring2026_Group1_Sections == null)
            {
                return NotFound();
            }
            var springSection = await _context.Spring2026_Group1_Sections.FindAsync(id);

            if (springSection != null)
            {
                Spring2026_Group1_Sections = springSection;
                _context.Spring2026_Group1_Sections.Remove(Spring2026_Group1_Sections);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
