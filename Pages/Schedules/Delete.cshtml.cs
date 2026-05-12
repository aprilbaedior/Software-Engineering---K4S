using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.Schedules
{
    public class DeleteModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DeleteModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
      public Spring2023_Group1_Schedules Spring2023_Group1_Schedules { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Spring2023_Group1_Schedules == null)
            {
                return NotFound();
            }

            var spring2023_group1_schedules = await _context.Spring2023_Group1_Schedules.FirstOrDefaultAsync(m => m.ScheduleID == id);

            if (spring2023_group1_schedules == null)
            {
                return NotFound();
            }
            else 
            {
                Spring2023_Group1_Schedules = spring2023_group1_schedules;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Spring2023_Group1_Schedules == null)
            {
                return NotFound();
            }
            var spring2023_group1_schedules = await _context.Spring2023_Group1_Schedules.FindAsync(id);

            if (spring2023_group1_schedules != null)
            {
                Spring2023_Group1_Schedules = spring2023_group1_schedules;
                _context.Spring2023_Group1_Schedules.Remove(Spring2023_Group1_Schedules);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
