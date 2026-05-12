using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.MySchedules
{
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public IList<Spring2026_Group1_Schedule> Spring2026_Group1_Schedule { get;set; } = default!;

        public async Task OnGetAsync()
        {
            // Fall back to logged-in user's email if session isn't set
            string studentEmail = HttpContext.Session.GetString("studentEmail")
                ?? User.Identity?.Name
                ?? string.Empty;

            if (!string.IsNullOrEmpty(studentEmail))
            {
                HttpContext.Session.SetString("studentEmail", studentEmail);
                Spring2026_Group1_Schedule = await _context.Spring2026_Group1_Schedule
                    .Where(s => s.StudentEmail == studentEmail)
                    .ToListAsync();
            }
            else
            {
                Spring2026_Group1_Schedule = new List<Spring2026_Group1_Schedule>();
            }
        }
    }
}
