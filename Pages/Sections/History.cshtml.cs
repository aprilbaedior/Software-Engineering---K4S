using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Sections
{
    [Authorize(Roles = "Instructor,Manager,Admin")]
    public class HistoryModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public HistoryModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public IList<Spring2026_Group1_Strike> Strikes { get; set; } = new List<Spring2026_Group1_Strike>();

        [BindProperty(SupportsGet = true)]
        public string? StudentEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StudentID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SectionID { get; set; }

        public string DisplayName { get; set; } = string.Empty;
        public string DisplayEmail { get; set; } = string.Empty;

        // New: basic profile / attendance properties
        public string? Sex { get; set; }
        public IList<Spring2025_Group3_Attendance> AttendanceRecords { get; set; } = new List<Spring2025_Group3_Attendance>();
        public int TotalSessions { get; set; }
        public int TotalSessionsPresent { get; set; }
        public double AttendancePercentage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(StudentEmail) && string.IsNullOrWhiteSpace(StudentID))
            {
                TempData["ErrorMessage"] = "Student identifier is required.";
                return Page();
            }

            var query = _context.Spring2026_Group1_Strike.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(StudentEmail))
            {
                var email = StudentEmail.Trim();
                query = query.Where(s => s.StudentEmail == email);
                DisplayEmail = email;
            }
            else if (int.TryParse(StudentID, out var sid))
            {
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
                DisplayEmail = first.StudentEmail ?? DisplayEmail;
                var profileName = await _context.Spring2023_Group1_Profile_Sys
                    .AsNoTracking()
                    .Where(p => p.Email == DisplayEmail)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();

                DisplayName = !string.IsNullOrWhiteSpace(profileName) ? profileName : (first.StudentName ?? DisplayEmail);
            }
            else
            {
                // fallback: resolve display name/email from profile or students table
                if (!string.IsNullOrWhiteSpace(DisplayEmail))
                {
                    var profileName = await _context.Spring2023_Group1_Profile_Sys
                        .AsNoTracking()
                        .Where(p => p.Email == DisplayEmail)
                        .Select(p => p.Name)
                        .FirstOrDefaultAsync();
                    DisplayName = profileName ?? DisplayEmail;
                }
                else if (int.TryParse(StudentID, out var sid2))
                {
                    var student = await _context.Spring2026_Group1_Students
                        .AsNoTracking()
                        .Where(s => s.StudentID == sid2)
                        .Select(s => new { s.Email })
                        .FirstOrDefaultAsync();

                    DisplayEmail = student?.Email ?? string.Empty;
                    var profileName = await _context.Spring2023_Group1_Profile_Sys
                        .AsNoTracking()
                        .Where(p => p.Email == DisplayEmail)
                        .Select(p => p.Name)
                        .FirstOrDefaultAsync();
                    DisplayName = profileName ?? DisplayEmail;
                }

                TempData["InfoMessage"] = "No disciplinary records found for this student.";
            }

            // New: Pull basic profile fields (Sex) and attendance for the specific section
            if (!string.IsNullOrWhiteSpace(DisplayEmail))
            {
                var profile = await _context.Spring2023_Group1_Profile_Sys
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Email == DisplayEmail);

                if (profile != null)
                {
                    // Sex stored as char? in profile; convert to readable string
                    Sex = profile.Sex?.ToString();
                }

                // load attendance records for this student (optionally filter by section)
                var attendanceQuery = _context.Spring2025_Group3_Attendance
                    .AsNoTracking()
                    .Where(a => a.Email == DisplayEmail);

                if (SectionID.HasValue)
                {
                    attendanceQuery = attendanceQuery.Where(a => a.SectionID == SectionID.Value);
                }

                AttendanceRecords = await attendanceQuery
                    .OrderByDescending(a => a.CurrentDate)
                    .ToListAsync();

                TotalSessions = AttendanceRecords.Count;
                TotalSessionsPresent = AttendanceRecords.Count(a => a.AttendanceStatus == "Present");

                AttendancePercentage = TotalSessions > 0
                    ? (double)TotalSessionsPresent / TotalSessions * 100.0
                    : 0.0;
            }

            return Page();
        }
    }
}