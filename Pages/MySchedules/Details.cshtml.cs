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

namespace CS395SI_Spring2023_Group1.Pages.MySchedules
{
    public class DetailsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public DetailsModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public Spring2024_Group2_Schedule Spring2024_Group2_Schedule { get; set; } = default!;
        
        // Attendance-related properties
        public IList<Spring2025_Group3_Attendance> AttendanceRecords { get; set; } = default!;
        public IList<Spring2025_Group3_Attendance> RecentAttendanceRecords { get; set; } = default!;
        public double PersonalAttendancePercentage { get; set; } = 0;
        public int TotalSessionsPresent { get; set; } = 0;
        public int TotalSessions { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Spring2024_Group2_Schedule == null)
            {
                return NotFound();
            }

            var studentEmail = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(studentEmail))
            {
                return RedirectToPage("/Account/Login");
            }

            var spring2024_group2_schedule = await _context.Spring2024_Group2_Schedule
                .FirstOrDefaultAsync(m => m.ScheduleID == id && m.StudentEmail == studentEmail);
                
            if (spring2024_group2_schedule == null)
            {
                return NotFound();
            }
            
            Spring2024_Group2_Schedule = spring2024_group2_schedule;
            
            AttendanceRecords = await _context.Spring2025_Group3_Attendance
                .Where(a => a.Email == studentEmail && 
                           a.SectionID == Spring2024_Group2_Schedule.SectionID)
                .OrderByDescending(a => a.CurrentDate)
                .ToListAsync();
                
            // Get recent records (last 5) for summary
            RecentAttendanceRecords = AttendanceRecords
                .OrderByDescending(a => a.CurrentDate)
                .Take(5)
                .OrderBy(a => a.CurrentDate)
                .ToList();
                
            TotalSessions = AttendanceRecords.Count;
            TotalSessionsPresent = AttendanceRecords.Count(a => a.AttendanceStatus == "Present");

            if (TotalSessions > 0)
            {
                PersonalAttendancePercentage = (double)TotalSessionsPresent / TotalSessions * 100;
            }
            
            return Page();
        }
    }
}