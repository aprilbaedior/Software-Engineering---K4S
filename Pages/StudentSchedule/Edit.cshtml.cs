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

namespace CS395SI_Spring2023_Group1.Pages.StudentSchedule
{
    public class EditModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public EditModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2024_Group2_Schedule Spring2024_Group2_Schedule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            // Prevent instructors from accessing this page
            if (User.IsInRole("Instructor"))
            {
                return Forbid();
            }

            if (id == null || _context.Spring2024_Group2_Schedule == null)
            {
                return NotFound();
            }

            var spring2024_group2_schedule =  await _context.Spring2024_Group2_Schedule.FirstOrDefaultAsync(m => m.ScheduleID == id);
            if (spring2024_group2_schedule == null)
            {
                return NotFound();
            }
            Spring2024_Group2_Schedule = spring2024_group2_schedule;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            // Prevent instructors from accessing this page
            if (User.IsInRole("Instructor"))
            {
                return Forbid();
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Spring2024_Group2_Schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Spring2024_Group2_ScheduleExists(Spring2024_Group2_Schedule.ScheduleID))
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

        private bool Spring2024_Group2_ScheduleExists(int id)
        {
          return (_context.Spring2024_Group2_Schedule?.Any(e => e.ScheduleID == id)).GetValueOrDefault();
        }
    }
}
