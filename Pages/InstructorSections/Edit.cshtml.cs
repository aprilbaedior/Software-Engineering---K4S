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

namespace CS395SI_Spring2023_Group1.Pages_InstructorSections
{
    public class EditModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public EditModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_Sections Spring2026_Group1_Sections { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spring2026_group1_sections =  await _context.Spring2026_Group1_Sections.FirstOrDefaultAsync(m => m.SectionID == id);
            if (spring2026_group1_sections == null)
            {
                return NotFound();
            }
            Spring2026_Group1_Sections = spring2026_group1_sections;
           ViewData["InstructorID"] = new SelectList(_context.Spring2026_Group1_Instructor, "InstructorID", "Email");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Spring2026_Group1_Sections).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Spring2026_Group1_SectionsExists(Spring2026_Group1_Sections.SectionID))
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

        private bool Spring2026_Group1_SectionsExists(int id)
        {
            return _context.Spring2026_Group1_Sections.Any(e => e.SectionID == id);
        }
    }
}
