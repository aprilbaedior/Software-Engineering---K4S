using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CS395SI_Spring2023_Group1.Pages.Instructors
{
    [Authorize(Roles = "Instructor")]
    public class DisplinaryHistoryModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly ILogger<DisplinaryHistoryModel> _logger;

        public DisplinaryHistoryModel(CS395SI_Spring2023_Group1Context context, ILogger<DisplinaryHistoryModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Spring2026_Group1_Strike> Strikes { get; set; } = new List<Spring2026_Group1_Strike>();

        // Query inputs (bound from query string)
        [BindProperty(SupportsGet = true)]
        public string? StudentID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StudentEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SectionID { get; set; }

        // Resolved display fields
        public string DisplayName { get; set; } = string.Empty;
        public string DisplayEmail { get; set; } = string.Empty;

        // Roster
        public List<StudentWithStrikes> StudentsInSection { get; set; } = new();

        // Available sections for dropdown
        public List<SelectListItem> AvailableSections { get; set; } = new();
        public bool HasMultipleSections { get; set; }

        public class StudentWithStrikes
        {
            public int? StudentID { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public int StrikeCount { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogDebug("DisplinaryHistory OnGetAsync called with StudentID={StudentID} StudentEmail={StudentEmail} SectionID={SectionID} User={User}",
                StudentID, StudentEmail, SectionID, User?.Identity?.Name);

            // Get all sections assigned to this instructor
            var userEmail = User?.Identity?.Name ?? string.Empty;
            List<int> assignedSectionIds = new();
            
            if (!string.IsNullOrEmpty(userEmail))
            {
                var normalizedUserEmail = userEmail.Trim().ToLower();
                assignedSectionIds = await _context.Spring2026_Group1_InstructorAssignment
                    .AsNoTracking()
                    .Where(a => a.InstructorEmail != null && a.InstructorEmail.Trim().ToLower() == normalizedUserEmail)
                    .Select(a => a.SectionID)
                    .Distinct()
                    .ToListAsync();

                // Load section details for dropdown
                if (assignedSectionIds.Any())
                {
                    var sections = await _context.Spring2026_Group1_Sections
                        .AsNoTracking()
                        .Where(s => assignedSectionIds.Contains(s.SectionID))
                        .OrderBy(s => s.ServiceName)
                        .ThenBy(s => s.StartTime)
                        .Select(s => new { s.SectionID, s.ServiceName, s.StartTime, s.WeekDay })
                        .ToListAsync();

                    AvailableSections = sections.Select(s => new SelectListItem
                    {
                        Value = s.SectionID.ToString(),
                        Text = $"{s.ServiceName} - Section {s.SectionID} ({s.WeekDay})",
                        Selected = s.SectionID == SectionID
                    }).ToList();

                    HasMultipleSections = AvailableSections.Count > 1;
                }
            }

            // If no SectionID provided, use the first assigned section (or prompt if multiple)
            if (!SectionID.HasValue && assignedSectionIds.Any())
            {
                if (assignedSectionIds.Count == 1)
                {
                    // Auto-select if only one section
                    SectionID = assignedSectionIds.First();
                }
                else
                {
                    // Multiple sections available - prompt user to select
                    TempData["InfoMessage"] = "Please select a section from the dropdown to view disciplinary history.";
                    return Page();
                }
            }

            // If we now have a SectionID, load the roster (students enrolled in that section)
            if (SectionID.HasValue)
            {
                await LoadRosterForSectionAsync(SectionID.Value);
            }

            // If a student identifier was provided, load their strikes (works with or without SectionID)
            if (!string.IsNullOrWhiteSpace(StudentEmail) || !string.IsNullOrWhiteSpace(StudentID))
            {
                await LoadStrikesForStudentAsync();
            }
            else if (!SectionID.HasValue)
            {
                // No section and no student supplied -> prompt
                TempData["ErrorMessage"] = "Provide a SectionID in the query string to list students, or provide StudentID/StudentEmail to view a student's history.";
            }

            return Page();
        }

        // Loads roster into StudentsInSection: reads enrolled emails from schedule, resolves names, counts strikes
        private async Task LoadRosterForSectionAsync(int sectionId)
        {
            // Get distinct student emails enrolled in this section from schedule table
            var enrolledEmails = await _context.Spring2024_Group2_Schedule
                .AsNoTracking()
                .Where(s => s.SectionID == sectionId)
                .Select(s => s.StudentEmail)
                .Distinct()
                .ToListAsync();

            if (enrolledEmails == null || enrolledEmails.Count == 0)
            {
                StudentsInSection = new List<StudentWithStrikes>();
                TempData["InfoMessage"] = "No students enrolled in this section.";
                return;
            }

            // Load profile names for enrolled emails (if available)
            var profiles = await _context.Spring2023_Group1_Profile_Sys
                .AsNoTracking()
                .Where(p => enrolledEmails.Contains(p.Email))
                .Select(p => new { p.Email, p.Name })
                .ToDictionaryAsync(p => p.Email, p => p.Name);

            // Load strikes for this section (group by email)
            var strikesGrouped = await _context.Spring2026_Group1_Strike
                .AsNoTracking()
                .Where(s => s.SectionID == sectionId)
                .GroupBy(s => s.StudentEmail)
                .Select(g => new { Email = g.Key, Count = g.Count() })
                .ToListAsync();

            var strikesDict = strikesGrouped
                .Where(x => !string.IsNullOrEmpty(x.Email))
                .ToDictionary(x => x.Email!, x => x.Count, StringComparer.OrdinalIgnoreCase);

            // Optionally resolve StudentID from Students table if you have it linked by email
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

        // Loads strikes into Strikes for the requested student
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

            if (SectionID.HasValue)
            {
                query = query.Where(s => s.SectionID == SectionID.Value);
            }

            Strikes = await query.OrderByDescending(s => s.FiledDate).ToListAsync();

            if (Strikes.Any())
            {
                var first = Strikes.First();
                DisplayEmail = first.StudentEmail ?? string.Empty;
                // Try profile name first
                var profileName = await _context.Spring2023_Group1_Profile_Sys
                    .AsNoTracking()
                    .Where(p => p.Email == DisplayEmail)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();

                DisplayName = !string.IsNullOrWhiteSpace(profileName) ? profileName : (first.StudentName ?? DisplayEmail);
            }
            else
            {
                // No strikes for the requested student — try to resolve email/name from Students or Profile table
                if (!string.IsNullOrWhiteSpace(StudentEmail))
                {
                    DisplayEmail = StudentEmail.Trim();
                    var profileName = await _context.Spring2023_Group1_Profile_Sys
                        .AsNoTracking()
                        .Where(p => p.Email == DisplayEmail)
                        .Select(p => p.Name)
                        .FirstOrDefaultAsync();
                    DisplayName = profileName ?? DisplayEmail;
                    TempData["InfoMessage"] = "No disciplinary records found for this student.";
                }
                else if (int.TryParse(StudentID, out var sid2))
                {
                    var student = await _context.Spring2026_Group1_Students
                        .AsNoTracking()
                        .Where(s => s.StudentID == sid2)
                        .Select(s => new { s.StudentID, s.Email })
                        .FirstOrDefaultAsync();

                    if (student != null)
                    {
                        DisplayEmail = student.Email ?? string.Empty;
                        var profileName = await _context.Spring2023_Group1_Profile_Sys
                            .AsNoTracking()
                            .Where(p => p.Email == DisplayEmail)
                            .Select(p => p.Name)
                            .FirstOrDefaultAsync();
                        DisplayName = profileName ?? DisplayEmail;
                        TempData["InfoMessage"] = "No disciplinary records found for this student.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No student record found for the provided Student ID.";
                    }
                }
            }
        }
    }
}