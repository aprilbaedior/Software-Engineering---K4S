using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;

namespace CS395SI_Spring2023_Group1.Pages.AttendanceForAdmin
{
    /// <summary>
    /// Page model for managing student attendance records by instructors.
    /// This page allows instructors to view, create, and update attendance for students enrolled in specific course sections.
    /// </summary>
    [Authorize(Roles = "Instructor,Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing attendance data.</param>
        public IndexModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets or sets the list of attendance records for display on the page.
        /// </summary>
        public IList<Spring2025_Group3_Attendance> AttendanceRecords { get; set; } = new List<Spring2025_Group3_Attendance>();
        
        /// <summary>
        /// Gets or sets the title of the current session being viewed.
        /// </summary>
        public string SessionTitle { get; set; } = "N/A";
        
        /// <summary>
        /// Gets or sets the scheduled time information for the section being viewed.
        /// </summary>
        public string ScheduleTime { get; set; } = "N/A";

        /// <summary>
        /// Gets or sets the section ID being viewed, bound from the query string.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int SectionID { get; set; }

        /// <summary>
        /// Gets or sets the status of the current operation or view.
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the week offset from the current date, bound from the query string.
        /// Used for navigating between different weeks of attendance records.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int WeekOffset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the start date of the week being viewed.
        /// </summary>
        public DateTime WeekStart { get; set; }
        public DateTime SessionDate { get; set; }           // first scheduled day (kept for compat)
        public List<DateTime> SessionDates { get; set; } = new(); // all scheduled days this week

        /// <summary>
        /// Gets or sets the percentage of class attendance for the current section and week.
        /// Calculated based on weighted attendance statuses.
        /// </summary>
        public double ClassAttendancePercentage { get; set; } = 0;

        /// <summary>
        /// Handles GET requests to load the attendance page for a specific section.
        /// Retrieves section details, enrolled students, and their attendance records for the selected week.
        /// Creates attendance records for days that don't have records yet.
        /// Calculates the overall class attendance percentage.
        /// </summary>
        /// <param name="sectionID">The ID of the section to view attendance for.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnGetAsync(int sectionID)
        {
            Console.WriteLine($"Section ID received: {sectionID}");

            SectionID = sectionID;

            // Calculate the start date of the selected week (based on week offset)
            WeekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek + (WeekOffset * 7));
            if (WeekStart.DayOfWeek != DayOfWeek.Monday)
            {
                WeekStart = WeekStart.AddDays(1);
            }

            // Retrieve section schedule details
            var sectionSchedule = await _context.Spring2026_Group1_Sections
                .Where(s => s.SectionID == sectionID)
                .Select(s => new
                {
                    s.ServiceName,
                    s.StartDate,
                    s.EndDate,
                    s.WeekDay,
                    s.StartTime,
                    s.EndTime
                })
                .FirstOrDefaultAsync();

            // Map day names to DayOfWeek enum
            var dayMap = new Dictionary<string, DayOfWeek>(StringComparer.OrdinalIgnoreCase)
            {
                { "monday",    DayOfWeek.Monday    },
                { "tuesday",   DayOfWeek.Tuesday   },
                { "wednesday", DayOfWeek.Wednesday },
                { "thursday",  DayOfWeek.Thursday  },
                { "friday",    DayOfWeek.Friday    },
                { "saturday",  DayOfWeek.Saturday  },
                { "sunday",    DayOfWeek.Sunday    }
            };

            // Parse all scheduled days from comma-separated WeekDay value
            var scheduledDayNames = (sectionSchedule?.WeekDay ?? "Monday")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim())
                .Where(d => dayMap.ContainsKey(d))
                .ToList();

            // Calculate a session date for each scheduled day within the current week
            // WeekStart is always Monday; offset each day from Monday
            SessionDates = scheduledDayNames
                .Select(d =>
                {
                    var dow = dayMap[d];
                    int daysFromMonday = dow == DayOfWeek.Sunday ? 6 : (int)dow - 1;
                    return WeekStart.AddDays(daysFromMonday);
                })
                .OrderBy(d => d)
                .ToList();

            // Keep singular SessionDate for backward compatibility (first scheduled day)
            SessionDate = SessionDates.FirstOrDefault();

            if (sectionSchedule != null)
            {
                var dayDisplay = string.Join(", ", scheduledDayNames);
                SessionTitle = $"{sectionSchedule.ServiceName} (Section {sectionID})";
                ScheduleTime = $"{dayDisplay} | {DateTime.Today.Add(sectionSchedule.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt")} - {DateTime.Today.Add(sectionSchedule.EndTime ?? TimeSpan.Zero).ToString("hh:mm tt")}";
            }

            if (_context.Spring2025_Group3_Attendance != null && _context.Spring2024_Group2_Schedule != null)
            {
                // Load records for ALL of the section's scheduled days this week
                var sessionDateValues = SessionDates.Select(d => d.Date).ToList();
                var existingAttendance = await _context.Spring2025_Group3_Attendance
                    .Where(a => a.SectionID == sectionID && sessionDateValues.Contains(a.CurrentDate.Date))
                    .ToListAsync();

                // Get all students enrolled in this section with their profile information
                var enrolledStudents = await (
                    from s in _context.Spring2024_Group2_Schedule
                    join p in _context.Spring2023_Group1_Profile_Sys
                    on s.StudentEmail equals p.Email
                    where s.SectionID == sectionID
                    select new
                    {
                        s.StudentEmail,
                        s.SectionID,
                        s.ServiceID,
                        s.ScheduleID,
                        p.Name
                    }).ToListAsync();

                // Create a dictionary mapping student emails to names for display
                ViewData["EmailToName"] = enrolledStudents.ToDictionary(e => e.StudentEmail, e => e.Name);

                // Create attendance records for any scheduled day that doesn't have records yet
                var newAttendanceRecords = new List<Spring2025_Group3_Attendance>();
                foreach (var student in enrolledStudents)
                {
                    // Verify student exists in profile table
                    bool existsInProfile = await _context.Spring2023_Group1_Profile_Sys
                        .AnyAsync(p => p.Email == student.StudentEmail);

                    if (!existsInProfile)
                    {
                        Console.WriteLine($"⚠ Skipping {student.StudentEmail} - Not found in Profile table!");
                        continue;
                    }

                    Console.WriteLine($"Processing Student: {student.StudentEmail} - ScheduleID: {student.ScheduleID}");

                    // Create a record for each scheduled day in the week that is missing
                    foreach (var sessionDate in SessionDates)
                    {
                        bool recordExists = existingAttendance
                            .Any(a => a.Email == student.StudentEmail && a.CurrentDate.Date == sessionDate.Date);

                        if (!recordExists)
                        {
                            newAttendanceRecords.Add(new Spring2025_Group3_Attendance
                            {
                                Email = student.StudentEmail,
                                SectionID = student.SectionID,
                                ServiceID = student.ServiceID,
                                ScheduleID = student.ScheduleID,
                                CurrentDate = sessionDate,
                                AttendanceStatus = "Not Marked"
                            });
                        }
                    }
                }

                // Bulk insert new records if any
                if (newAttendanceRecords.Any())
                {
                    _context.Spring2025_Group3_Attendance.AddRange(newAttendanceRecords);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"✓ Created {newAttendanceRecords.Count} new attendance record(s).");

                    // Refresh the list of existing attendance records for all scheduled days
                    existingAttendance = await _context.Spring2025_Group3_Attendance
                        .Where(a => a.SectionID == sectionID && sessionDateValues.Contains(a.CurrentDate.Date))
                        .ToListAsync();
                }

                // Load records for all scheduled days this week
                AttendanceRecords = await _context.Spring2025_Group3_Attendance
                    .Where(a => a.SectionID == sectionID && sessionDateValues.Contains(a.CurrentDate.Date))
                    .OrderBy(a => a.Email)
                    .ToListAsync();

                // Calculate class-wide attendance percentage for the session
                if (AttendanceRecords.Any())
                {
                    int totalRecords = AttendanceRecords.Count;
                    double weightedSum =
                        AttendanceRecords.Count(a => a.AttendanceStatus == "Present") * 1.0 +
                        AttendanceRecords.Count(a => a.AttendanceStatus == "Late") * 0.8 +
                        AttendanceRecords.Count(a => a.AttendanceStatus == "Excused") * 0.5 +
                        AttendanceRecords.Count(a => a.AttendanceStatus == "Absent") * 0.0;

                    ClassAttendancePercentage = (weightedSum / totalRecords) * 100.0;
                }
            }
        }

        /// <summary>
        /// Handles POST requests to update attendance status for a student on a specific date.
        /// </summary>
        /// <param name="email">The email of the student whose attendance is being updated.</param>
        /// <param name="date">The date for which the attendance is being recorded.</param>
        /// <param name="status">The attendance status (e.g., Present, Absent, Late, Excused).</param>
        /// <returns>A JSON result indicating success or failure of the update operation.</returns>
        public async Task<IActionResult> OnPostPostAttendanceAsync(string email, string date, string status)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(status))
            {
                return new JsonResult(new { success = false, message = "Missing required parameters" });
            }

            DateTime attendanceDate;
            if (!DateTime.TryParse(date, out attendanceDate))
            {
                return new JsonResult(new { success = false, message = "Invalid date format" });
            }

            var record = await _context.Spring2025_Group3_Attendance
                .FirstOrDefaultAsync(a =>
                    a.Email == email &&
                    a.CurrentDate.Date == attendanceDate.Date &&
                    a.SectionID == SectionID);

            if (record == null)
            {
                return new JsonResult(new { success = false, message = "Attendance record not found" });
            }

            record.AttendanceStatus = status;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Attendance updated successfully" });
        }
    }
}