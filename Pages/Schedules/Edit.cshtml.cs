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

namespace CS395SI_Spring2023_Group1.Pages.Schedules
{
    public class EditModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public EditModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
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

            var spring2023_group1_schedules =  await _context.Spring2023_Group1_Schedules.FirstOrDefaultAsync(m => m.ScheduleID == id);
            if (spring2023_group1_schedules == null)
            {
                return NotFound();
            }
            Spring2023_Group1_Schedules = spring2023_group1_schedules;
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

            _context.Attach(Spring2023_Group1_Schedules).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Spring2023_Group1_SchedulesExists(Spring2023_Group1_Schedules.ScheduleID))
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

        private bool Spring2023_Group1_SchedulesExists(int id)
        {
          return (_context.Spring2023_Group1_Schedules?.Any(e => e.ScheduleID == id)).GetValueOrDefault();
        }
    }
}
