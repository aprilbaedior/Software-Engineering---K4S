using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.AttendanceForAdmin
{
    [Authorize(Roles = "Instructor,Manager,Admin")]
    public class SelectSectionModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public SelectSectionModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2026_Group1_Sections> AssignedSections { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                AssignedSections = new List<Spring2026_Group1_Sections>();
                return;
            }

            // Get section IDs assigned to this instructor
            var sectionIds = await _context.Spring2026_Group1_InstructorAssignment
                .AsNoTracking()
                .Where(a => a.InstructorEmail == userEmail)
                .Select(a => a.SectionID)
                .ToListAsync();

            // Get full section details
            AssignedSections = await _context.Spring2026_Group1_Sections
                .AsNoTracking()
                .Where(s => sectionIds.Contains(s.SectionID))
                .OrderBy(s => s.ServiceName)
                .ThenBy(s => s.WeekDay)
                .ToListAsync();
        }
    }
}