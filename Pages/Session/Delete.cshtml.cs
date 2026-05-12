using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.Session
{
    public class DeleteModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DeleteModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
      public Spring2024_Group2_Session Spring2024_Group2_Session { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Spring2024_Group2_Session == null)
            {
                return NotFound();
            }

            var spring2024_group2_session = await _context.Spring2024_Group2_Session.FirstOrDefaultAsync(m => m.SessionID == id);

            if (spring2024_group2_session == null)
            {
                return NotFound();
            }
            else 
            {
                Spring2024_Group2_Session = spring2024_group2_session;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Spring2024_Group2_Session == null)
            {
                return NotFound();
            }
            var spring2024_group2_session = await _context.Spring2024_Group2_Session.FindAsync(id);

            if (spring2024_group2_session != null)
            {
                Spring2024_Group2_Session = spring2024_group2_session;
                _context.Spring2024_Group2_Session.Remove(Spring2024_Group2_Session);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
