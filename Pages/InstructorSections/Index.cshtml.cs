using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.InstructorSections
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Spring2026_Group1_Sections> AssignedSections { get; set; } = default!;
        public Spring2026_Group1_Instructor? CurrentInstructor { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && _context.Spring2026_Group1_Instructor != null)
            {
                // Find the instructor record by email
                CurrentInstructor = await _context.Spring2026_Group1_Instructor
                    .FirstOrDefaultAsync(i => i.Email == user.Email);

                if (CurrentInstructor != null && _context.Spring2026_Group1_Sections != null)
                {
                    // Filter sections by instructor's ID
                    AssignedSections = await _context.Spring2026_Group1_Sections
                        .Where(s => s.InstructorID == CurrentInstructor.InstructorID)
                        .OrderBy(s => s.ServiceName)
                        .ThenBy(s => s.StartTime)
                        .ToListAsync();
                }
                else
                {
                    AssignedSections = new List<Spring2026_Group1_Sections>();
                }
            }
            else
            {
                AssignedSections = new List<Spring2026_Group1_Sections>();
            }
        }
    }
}
