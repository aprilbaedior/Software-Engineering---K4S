using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Manager.Students
{
    [Authorize(Roles = "Manager,Admin")]
    public class ProfileModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public ProfileModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public Spring2023_Group1_Profile_Sys? Student { get; set; }

        public class EnrolledSection
        {
            public int SectionID { get; set; }
            public string ServiceName { get; set; } = string.Empty;
            public string WeekDay { get; set; } = string.Empty;
            public TimeSpan? StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public int PresentCount { get; set; }
            public int LateCount { get; set; }
            public int ExcusedCount { get; set; }
            public int AbsentCount { get; set; }
            public int NotMarkedCount { get; set; }
            public double AttendancePercent { get; set; }
            public bool HasCertificate { get; set; }
        }

        public List<EnrolledSection> EnrolledSections { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Student = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(p => p.Email == id);

            if (Student == null)
                return NotFound();

            // Load enrolled sections with attendance stats
            var schedules = await _context.Spring2024_Group2_Schedule!
                .Where(s => s.StudentEmail == id)
                .ToListAsync();

            var sectionIds = schedules.Select(s => s.SectionID).ToList();

            var attendanceRecords = await _context.Spring2025_Group3_Attendance!
                .Where(a => a.Email == id && sectionIds.Contains(a.SectionID))
                .ToListAsync();

            var certificates = await _context.Spring2026_Group1_Certificate!
                .Where(c => c.StudentEmail == id)
                .Select(c => c.SectionID)
                .ToListAsync();

            foreach (var sched in schedules)
            {
                var records = attendanceRecords.Where(a => a.SectionID == sched.SectionID).ToList();
                int totalMarked = records.Count(a => a.AttendanceStatus != "Not Marked");

                double pct = 0;
                if (totalMarked > 0)
                {
                    double weighted =
                        records.Count(a => a.AttendanceStatus == "Present") * 1.0 +
                        records.Count(a => a.AttendanceStatus == "Late") * 0.8 +
                        records.Count(a => a.AttendanceStatus == "Excused") * 0.5;
                    pct = Math.Round((weighted / totalMarked) * 100, 1);
                }

                EnrolledSections.Add(new EnrolledSection
                {
                    SectionID = sched.SectionID,
                    ServiceName = sched.ServiceName ?? "Unknown",
                    WeekDay = sched.WeekDay,
                    StartTime = sched.StartTime,
                    EndTime = sched.EndTime,
                    StartDate = sched.StartDate,
                    EndDate = sched.EndDate,
                    Status = sched.Status,
                    PresentCount = records.Count(a => a.AttendanceStatus == "Present"),
                    LateCount = records.Count(a => a.AttendanceStatus == "Late"),
                    ExcusedCount = records.Count(a => a.AttendanceStatus == "Excused"),
                    AbsentCount = records.Count(a => a.AttendanceStatus == "Absent"),
                    NotMarkedCount = records.Count(a => a.AttendanceStatus == "Not Marked"),
                    AttendancePercent = pct,
                    HasCertificate = certificates.Contains(sched.SectionID)
                });
            }

            return Page();
        }
    }
}
