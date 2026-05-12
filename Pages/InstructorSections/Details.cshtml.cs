using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages_InstructorSections
{
    public class DetailsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DetailsModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public Spring2026_Group1_Sections Spring2026_Group1_Sections { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spring2026_group1_sections = await _context.Spring2026_Group1_Sections.FirstOrDefaultAsync(m => m.SectionID == id);

            if (spring2026_group1_sections is not null)
            {
                Spring2026_Group1_Sections = spring2026_group1_sections;

                return Page();
            }

            return NotFound();
        }
    }
}
