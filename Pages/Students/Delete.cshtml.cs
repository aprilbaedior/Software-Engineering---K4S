using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages_Students
{
    public class DeleteModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DeleteModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_Students Spring2026_Group1_Students { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spring2026_group1_students = await _context.Spring2026_Group1_Students.FirstOrDefaultAsync(m => m.StudentID == id);

            if (spring2026_group1_students is not null)
            {
                Spring2026_Group1_Students = spring2026_group1_students;

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

            var spring2026_group1_students = await _context.Spring2026_Group1_Students.FindAsync(id);
            if (spring2026_group1_students != null)
            {
                Spring2026_Group1_Students = spring2026_group1_students;
                _context.Spring2026_Group1_Students.Remove(Spring2026_Group1_Students);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
