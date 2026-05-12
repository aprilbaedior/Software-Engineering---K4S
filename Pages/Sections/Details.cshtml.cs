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
    [Authorize(Roles = "Manager,Admin, Instructor")]
    public class DetailsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public DetailsModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public Spring2026_Group1_Sections Spring2026_Group1_Sections { get; set; } = default!;
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceID { get; set; } = string.Empty;
        public string AssignedInstructor { get; set; } = string.Empty;
        public string AssignedInstructorEmail { get; set; } = string.Empty;

        // New Properties used to tailor UI for instructors
        public bool IsCurrentUserInstructor { get; set; } = false;
        public bool IsInstructorAssignedToThisSection { get; set; } = false;
        public string CurrentUserEmail { get; set; } = string.Empty;

        // Embedded roster + student disciplinary history support
        public class StudentWithStrikes
        {
            public int? StudentID { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public int StrikeCount { get; set; }
        }

        public List<StudentWithStrikes> StudentsInSection { get; set; } = new();
        public IList<Spring2026_Group1_Strike> Strikes { get; set; } = new List<Spring2026_Group1_Strike>();

        // Query inputs (allow selecting a student on this page)
        [BindProperty(SupportsGet = true)]
        public string? StudentEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StudentID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Spring2026_Group1_Sections == null)
            {
                return NotFound();
            }

            var section = await _context.Spring2026_Group1_Sections
                .FirstOrDefaultAsync(m => m.SectionID == id);

            if (section == null)
            {
                return NotFound();
            }

            Spring2026_Group1_Sections = section;
            ServiceID = section.ServiceID;

            // Fetch the service name
            ServiceName = await _context.Spring2023_Group1_Services
                .Where(s => s.ServiceID == section.ServiceID)
                .Select(s => s.ServiceName)
                .FirstOrDefaultAsync() ?? "Unknown Service";

            // Fetch assigned instructor
            var assignment = await _context.Spring2026_Group1_InstructorAssignment
                .FirstOrDefaultAsync(a => a.SectionID == id);

            if (assignment != null)
            {
                AssignedInstructor = assignment.InstructorName;
                AssignedInstructorEmail = assignment.InstructorEmail;
            }

            // Current user info & instructor checks
            CurrentUserEmail = User.Identity?.Name ?? string.Empty;
            IsCurrentUserInstructor = User?.IsInRole("Instructor") ?? false;
            IsInstructorAssignedToThisSection = IsCurrentUserInstructor && assignment != null && !string.IsNullOrEmpty(assignment.InstructorEmail) &&
                string.Equals(assignment.InstructorEmail.Trim(), CurrentUserEmail.Trim(), StringComparison.OrdinalIgnoreCase);

            // Load roster for this section (embedded view)
            if (Spring2026_Group1_Sections.SectionID != 0)
            {
                await LoadRosterForSectionAsync(Spring2026_Group1_Sections.SectionID);
            }

            // If a student was selected on the querystring, load their strikes and display under the roster
            if (!string.IsNullOrWhiteSpace(StudentEmail) || !string.IsNullOrWhiteSpace(StudentID))
            {
                await LoadStrikesForStudentAsync();
            }

            return Page();
        }

        private async Task LoadRosterForSectionAsync(int sectionId)
        {
            var enrolledEmails = await _context.Spring2024_Group2_Schedule
                .AsNoTracking()
                .Where(s => s.SectionID == sectionId)
                .Select(s => s.StudentEmail)
                .Distinct()
                .ToListAsync();

            if (enrolledEmails == null || enrolledEmails.Count == 0)
            {
                StudentsInSection = new List<StudentWithStrikes>();
                return;
            }

            var profiles = await _context.Spring2023_Group1_Profile_Sys
                .AsNoTracking()
                .Where(p => enrolledEmails.Contains(p.Email))
                .Select(p => new { p.Email, p.Name })
                .ToDictionaryAsync(p => p.Email, p => p.Name);

            var strikesGrouped = await _context.Spring2026_Group1_Strike
                .AsNoTracking()
                .Where(s => s.SectionID == sectionId)
                .GroupBy(s => s.StudentEmail)
                .Select(g => new { Email = g.Key, Count = g.Count() })
                .ToListAsync();

            var strikesDict = strikesGrouped
                .Where(x => !string.IsNullOrEmpty(x.Email))
                .ToDictionary(x => x.Email!, x => x.Count, StringComparer.OrdinalIgnoreCase);

            var studentsByEmail = await _context.Spring2026_Group1_Students
                .AsNoTracking()
                .Where(s => enrolledEmails.Contains(s.Email))
                .Select(s => new { s.Email, s.StudentID })
                .ToDictionaryAsync(s => s.Email, s => s.StudentID);

            StudentsInSection = enrolledEmails
                .Select(email => new StudentWithStrikes
                {
                    Email = email ?? string.Empty,
                    Name = profiles.ContainsKey(email) && !string.IsNullOrWhiteSpace(profiles[email]) ? profiles[email] : (email ?? string.Empty),
                    StrikeCount = strikesDict.TryGetValue(email ?? string.Empty, out var cnt) ? cnt : 0,
                    StudentID = studentsByEmail.TryGetValue(email ?? string.Empty, out var sid) ? sid : null
                })
                .OrderByDescending(s => s.StrikeCount)
                .ThenBy(s => s.Name)
                .ToList();
        }

        private async Task LoadStrikesForStudentAsync()
        {
            var query = _context.Spring2026_Group1_Strike.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(StudentEmail))
            {
                var email = StudentEmail.Trim();
                query = query.Where(s => s.StudentEmail == email);
            }
            else
            {
                if (!int.TryParse(StudentID, out var sid))
                {
                    TempData["ErrorMessage"] = "Student ID must be numeric.";
                    return;
                }
                query = query.Where(s => s.StudentID == sid);
            }

            // Restrict to this section (optional)
            if (Spring2026_Group1_Sections.SectionID != 0)
            {
                query = query.Where(s => s.SectionID == Spring2026_Group1_Sections.SectionID);
            }

            Strikes = await query.OrderByDescending(s => s.FiledDate).ToListAsync();

            if (!Strikes.Any())
            {
                TempData["InfoMessage"] = "No disciplinary records found for this student.";
            }
        }
    }
}