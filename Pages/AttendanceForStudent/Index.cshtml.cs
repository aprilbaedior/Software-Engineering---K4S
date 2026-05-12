using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using System.Security.Claims;

namespace CS395SI_Spring2023_Group1.Pages.AttendanceForStudent
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public IList<Spring2025_Group3_Attendance> AttendanceRecords { get; set; } = default!;
        public string? ServiceName { get; set; }
        public string? ScheduleTime { get; set; }
        public string? StudentEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ScheduleID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int WeekOffset { get; set; } = 0;

        public DateTime WeekStart { get; set; }
        
        public int SectionID { get; set; }
        public string? ServiceID { get; set; }
        
        public double PersonalAttendancePercentage { get; set; } = 0;
        public int TotalSessionsPresent { get; set; } = 0;
        public int TotalSessions { get; set; } = 0;
        
        public bool IsAdmin { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ScheduleID = id;

            // Check if user is an admin - keeping this for functionality
            IsAdmin = User.IsInRole("Instructor");

            // If no schedule ID provided, try to find one for the logged-in student
            if (ScheduleID == 0)
            {
                var email = User.Identity?.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    var first = await _context.Spring2024_Group2_Schedule
                        .FirstOrDefaultAsync(s => s.StudentEmail == email);
                    if (first != null)
                        ScheduleID = first.ScheduleID;
                }
                if (ScheduleID == 0)
                    return RedirectToPage("/MySchedules/Index");
            }

            // Get schedule information
            var schedule = await _context.Spring2024_Group2_Schedule
                .FirstOrDefaultAsync(s => s.ScheduleID == ScheduleID);

            if (schedule == null)
            {
                return RedirectToPage("/MySchedules/Index");
            }
            
            // Set the student email based on the schedule
            StudentEmail = schedule.StudentEmail;
            Console.WriteLine($"StudentEmail from schedule: {StudentEmail}");

            // Removed the authorization check that was preventing access

            SectionID = schedule.SectionID;
            ServiceID = schedule.ServiceID;
            Console.WriteLine($"SectionID: {SectionID}, ServiceID: {ServiceID}");

            // Calculate the start of the current week (or offset week)
            WeekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek + (WeekOffset * 7));
            if (WeekStart.DayOfWeek != DayOfWeek.Monday)
            {
                WeekStart = WeekStart.AddDays(1); 
            }
            Console.WriteLine($"WeekStart: {WeekStart}");

            // Get section information
            var sectionInfo = await _context.Spring2026_Group1_Sections
                .Where(s => s.SectionID == SectionID)
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

            Console.WriteLine($"Section info found: {sectionInfo != null}");

            if (sectionInfo != null)
            {
                ServiceName = sectionInfo.ServiceName;
                ScheduleTime = $"{sectionInfo.WeekDay} | {sectionInfo.StartTime} - {sectionInfo.EndTime}";
                Console.WriteLine($"Service Name: {ServiceName}, Schedule Time: {ScheduleTime}");
            }

            // Get attendance records for the student for the current week
            AttendanceRecords = await _context.Spring2025_Group3_Attendance
                .Where(a => a.Email == StudentEmail && 
                           a.SectionID == SectionID &&
                           a.CurrentDate >= WeekStart && 
                           a.CurrentDate < WeekStart.AddDays(7))
                .OrderBy(a => a.CurrentDate)
                .ToListAsync();

            Console.WriteLine($"Attendance records found: {AttendanceRecords.Count}");

            // If no records exist for this week, create placeholders but don't save them
            var placeholders = new List<Spring2025_Group3_Attendance>();
            
            if (!AttendanceRecords.Any() && sectionInfo != null)
            {
                Console.WriteLine("Creating placeholder records");
                for (int i = 0; i < 7; i++)
                {
                    var currentDate = WeekStart.AddDays(i);
                    
                    // Check if this is a day the class meets
                    if (currentDate.DayOfWeek.ToString() == sectionInfo.WeekDay)
                    {
                        Console.WriteLine($"Adding placeholder for {currentDate.DayOfWeek}");
                        placeholders.Add(new Spring2025_Group3_Attendance
                        {
                            Email = StudentEmail,
                            SectionID = SectionID,
                            ServiceID = ServiceID,
                            ScheduleID = ScheduleID,
                            CurrentDate = currentDate,
                            AttendanceStatus = "Not Marked"
                        });
                    }
                }
                
                // Just use the placeholders for display, don't save to DB
                AttendanceRecords = placeholders;
                Console.WriteLine($"Created {placeholders.Count} placeholder records");
            }

            // Calculate attendance percentage
            // Get all attendance records for this student in this section
            var allAttendanceRecords = await _context.Spring2025_Group3_Attendance
                .Where(a => a.Email == StudentEmail && 
                           a.SectionID == SectionID)
                .ToListAsync();

            TotalSessions = allAttendanceRecords.Count;
            TotalSessionsPresent = allAttendanceRecords.Count(a => a.AttendanceStatus == "Present");
            Console.WriteLine($"Total sessions: {TotalSessions}, Present: {TotalSessionsPresent}");

            if (TotalSessions > 0)
            {
                PersonalAttendancePercentage = (double)TotalSessionsPresent / TotalSessions * 100;
                Console.WriteLine($"Attendance percentage: {PersonalAttendancePercentage}%");
            }

            Console.WriteLine("OnGetAsync completed successfully");
            return Page();
        }
    }
}