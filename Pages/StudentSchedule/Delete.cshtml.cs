using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.StudentSchedule
{
    public class DeleteModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DeleteModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
      public Spring2024_Group2_Schedule Spring2024_Group2_Schedule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Spring2024_Group2_Schedule == null)
            {
                return NotFound();
            }

            var spring2024_group2_schedule = await _context.Spring2024_Group2_Schedule.FirstOrDefaultAsync(m => m.ScheduleID == id);

            if (spring2024_group2_schedule == null)
            {
                return NotFound();
            }
            else 
            {
                Spring2024_Group2_Schedule = spring2024_group2_schedule;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Spring2024_Group2_Schedule == null)
            {
                return NotFound();
            }
            var spring2024_group2_schedule = await _context.Spring2024_Group2_Schedule.FindAsync(id);

            if (spring2024_group2_schedule != null)
            {
                Spring2024_Group2_Schedule = spring2024_group2_schedule;
                _context.Spring2024_Group2_Schedule.Remove(Spring2024_Group2_Schedule);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
