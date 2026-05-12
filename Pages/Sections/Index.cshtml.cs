using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Sections
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<SectionWithInstructor> Spring2026_Group1_Sections { get; set; } = new();
        public string ServiceID { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string serviceID)
        {
            ServiceID = serviceID?.Trim() ?? string.Empty;

            // Get sections with instructor assignments
            var sectionsQuery = _context.Spring2026_Group1_Sections.AsQueryable();

            if (!string.IsNullOrEmpty(serviceID))
            {
                var trimmedServiceID = serviceID.Trim();
                sectionsQuery = sectionsQuery.Where(s => s.ServiceID.Trim() == trimmedServiceID);
                ServiceName = await _context.Spring2023_Group1_Services
                    .Where(s => s.ServiceID.Trim() == trimmedServiceID)
                    .Select(s => s.ServiceName)
                    .FirstOrDefaultAsync() ?? "Unknown Service";
            }
            else
            {
                ServiceName = "All Services";
            }

            // Load sections with instructor data
            Spring2026_Group1_Sections = await sectionsQuery
                .Select(s => new SectionWithInstructor
                {
                    SectionID = s.SectionID,
                    ServiceID = s.ServiceID,
                    ServiceName = s.ServiceName,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    WeekDay = s.WeekDay,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status,
                    Capacity = s.Capacity,
                    InstructorID = s.InstructorID,
                    // Get instructor name from assignment table
                    InstructorName = _context.Spring2026_Group1_InstructorAssignment
                        .Where(a => a.SectionID == s.SectionID)
                        .Select(a => a.InstructorName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Page();
        }

        public IActionResult OnPost(string? sectionID)
        {
            if (string.IsNullOrEmpty(sectionID))
            {
                TempData["ErrorMessage"] = "Section ID is required.";
                return Page();
            }

            HttpContext.Session.SetString("sectionID", sectionID);
            return RedirectToPage("/AttendanceForAdmin/Index", new { sectionID });
        }

        // Helper class to hold section with instructor data
        public class SectionWithInstructor
        {
            public int SectionID { get; set; }
            public string? ServiceID { get; set; }
            public string? ServiceName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? WeekDay { get; set; }
            public TimeSpan? StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }
            public string? Status { get; set; }
            public int? Capacity { get; set; }
            public int? InstructorID { get; set; }
            public string? InstructorName { get; set; }
        }
    }
}