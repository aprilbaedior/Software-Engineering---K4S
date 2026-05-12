using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Sections
{
    [Authorize(Roles = "Manager,Admin")]
    public class EditModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public EditModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_Sections Spring2026_Group1_Sections { get; set; } = default!;

        [BindProperty]
        public List<string> SelectedDays { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Spring2026_Group1_Sections == null)
            {
                return NotFound();
            }

            var springSection =  await _context.Spring2026_Group1_Sections.FirstOrDefaultAsync(m => m.SectionID == id);
            if (springSection == null)
            {
                return NotFound();
            }
            Spring2026_Group1_Sections = springSection;

            // Pre-populate SelectedDays from saved comma-separated WeekDay value
            if (!string.IsNullOrEmpty(springSection.WeekDay))
            {
                SelectedDays = springSection.WeekDay
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Spring2026_Group1_Sections.Instructor");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var section = await _context.Spring2026_Group1_Sections!
                .FirstOrDefaultAsync(s => s.SectionID == Spring2026_Group1_Sections.SectionID);

            if (section == null)
            {
                return NotFound();
            }

            // Update only the editable fields — never touch InstructorID/navigation props
            section.ServiceID   = Spring2026_Group1_Sections.ServiceID;
            section.ServiceName = Spring2026_Group1_Sections.ServiceName;
            section.StartDate   = Spring2026_Group1_Sections.StartDate;
            section.EndDate     = Spring2026_Group1_Sections.EndDate;
            section.WeekDay     = SelectedDays.Any()
                ? string.Join(",", SelectedDays)
                : null;
            section.StartTime   = Spring2026_Group1_Sections.StartTime;
            section.EndTime     = Spring2026_Group1_Sections.EndTime;
            section.Status      = Spring2026_Group1_Sections.Status;

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
          return (_context.Spring2026_Group1_Sections?.Any(e => e.SectionID == id)).GetValueOrDefault();
        }
    }
}
