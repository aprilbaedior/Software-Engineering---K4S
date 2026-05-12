using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages_Managers
{
    public class DeleteModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DeleteModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_Manager Spring2026_Group1_Manager { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spring2026_group1_manager = await _context.Spring2026_Group1_Manager.FirstOrDefaultAsync(m => m.ManagerID == id);

            if (spring2026_group1_manager is not null)
            {
                Spring2026_Group1_Manager = spring2026_group1_manager;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spring2026_group1_manager = await _context.Spring2026_Group1_Manager.FindAsync(id);
            if (spring2026_group1_manager != null)
            {
                Spring2026_Group1_Manager = spring2026_group1_manager;
                _context.Spring2026_Group1_Manager.Remove(Spring2026_Group1_Manager);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
