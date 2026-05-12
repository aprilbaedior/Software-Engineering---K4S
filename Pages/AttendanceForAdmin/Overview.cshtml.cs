using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.AttendanceForAdmin
{
    [Authorize(Roles = "Admin,SuperAdmin,Manager")]
    public class OverviewModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public OverviewModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        // Filter Properties
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ServiceFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SectionFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? InstructorFilter { get; set; }

        // Statistics
        public int TotalAttendanceRecords { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public int TotalExcused { get; set; }
        public double OverallAttendanceRate { get; set; }

        // Data Collections
        public List<ServiceAttendanceStats> ServiceStats { get; set; } = new List<ServiceAttendanceStats>();
        public List<SectionAttendanceStats> SectionStats { get; set; } = new List<SectionAttendanceStats>();
        public List<StudentAttendanceSummary> StudentsWithPoorAttendance { get; set; } = new List<StudentAttendanceSummary>();
        public List<DailyAttendanceTrend> AttendanceTrends { get; set; } = new List<DailyAttendanceTrend>();
        
        // Filter Options
        public List<Spring2023_Group1_Services> AvailableServices { get; set; } = new List<Spring2023_Group1_Services>();
        public List<Spring2026_Group1_Sections> AvailableSections { get; set; } = new List<Spring2026_Group1_Sections>();
        public List<string> AvailableInstructors { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            // Set default date range if not specified (last 30 days)
            if (!StartDate.HasValue)
                StartDate = DateTime.Today.AddDays(-30);
            
            if (!EndDate.HasValue)
                EndDate = DateTime.Today;

            // Load filter options
            AvailableServices = await _context.Spring2023_Group1_Services.ToListAsync();
            AvailableSections = await _context.Spring2026_Group1_Sections.ToListAsync();
            AvailableInstructors = await _context.Spring2026_Group1_InstructorAssignment
                .Select(ia => ia.InstructorEmail)
                .Distinct()
                .ToListAsync();

            // Build base query with filters
            var attendanceQuery = _context.Spring2025_Group3_Attendance
                .Where(a => a.CurrentDate >= StartDate && a.CurrentDate <= EndDate);

            if (!string.IsNullOrEmpty(ServiceFilter))
                attendanceQuery = attendanceQuery.Where(a => a.ServiceID == ServiceFilter);

            if (SectionFilter.HasValue)
                attendanceQuery = attendanceQuery.Where(a => a.SectionID == SectionFilter.Value);

            if (!string.IsNullOrEmpty(InstructorFilter))
            {
                var instructorSections = await _context.Spring2026_Group1_InstructorAssignment
                    .Where(ia => ia.InstructorEmail == InstructorFilter)
                    .Select(ia => ia.SectionID)
                    .ToListAsync();
                attendanceQuery = attendanceQuery.Where(a => instructorSections.Contains(a.SectionID));
            }

            var attendanceRecords = await attendanceQuery.ToListAsync();

            // Calculate overall statistics
            TotalAttendanceRecords = attendanceRecords.Count;
            TotalPresent = attendanceRecords.Count(a => a.AttendanceStatus == "Present");
            TotalAbsent = attendanceRecords.Count(a => a.AttendanceStatus == "Absent");
            TotalLate = attendanceRecords.Count(a => a.AttendanceStatus == "Late");
            TotalExcused = attendanceRecords.Count(a => a.AttendanceStatus == "Excused");

            if (TotalAttendanceRecords > 0)
            {
                // Calculate weighted attendance rate (Present=1.0, Late=0.5, Excused=0, Absent=0)
                double weightedAttendance = TotalPresent + (TotalLate * 0.5);
                OverallAttendanceRate = (weightedAttendance / TotalAttendanceRecords) * 100;
            }

            // Calculate service-level statistics
            var serviceGroups = attendanceRecords.GroupBy(a => a.ServiceID);
            foreach (var group in serviceGroups)
            {
                var service = await _context.Spring2023_Group1_Services
                    .FirstOrDefaultAsync(s => s.ServiceID == group.Key);

                if (service != null)
                {
                    int present = group.Count(a => a.AttendanceStatus == "Present");
                    int late = group.Count(a => a.AttendanceStatus == "Late");
                    int total = group.Count();
                    double rate = total > 0 ? ((present + (late * 0.5)) / total) * 100 : 0;

                    ServiceStats.Add(new ServiceAttendanceStats
                    {
                        ServiceName = service.ServiceName,
                        TotalSessions = total,
                        PresentCount = present,
                        AbsentCount = group.Count(a => a.AttendanceStatus == "Absent"),
                        LateCount = late,
                        AttendanceRate = rate
                    });
                }
            }

            // Calculate section-level statistics
            var sectionGroups = attendanceRecords.GroupBy(a => a.SectionID);
            foreach (var group in sectionGroups)
            {
                var section = await _context.Spring2026_Group1_Sections
                    .FirstOrDefaultAsync(s => s.SectionID == group.Key);

                if (section != null)
                {
                    var instructor = await _context.Spring2026_Group1_InstructorAssignment
                        .FirstOrDefaultAsync(ia => ia.SectionID == group.Key);

                    int present = group.Count(a => a.AttendanceStatus == "Present");
                    int late = group.Count(a => a.AttendanceStatus == "Late");
                    int total = group.Count();
                    double rate = total > 0 ? ((present + (late * 0.5)) / total) * 100 : 0;

                    SectionStats.Add(new SectionAttendanceStats
                    {
                        SectionID = section.SectionID,
                        SectionName = $"{section.ServiceID} - Section {section.SectionID}",
                        InstructorEmail = instructor?.InstructorEmail ?? "Unassigned",
                        TotalSessions = total,
                        PresentCount = present,
                        AbsentCount = group.Count(a => a.AttendanceStatus == "Absent"),
                        AttendanceRate = rate
                    });
                }
            }

            // Identify students with poor attendance (< 75%)
            var studentGroups = attendanceRecords.GroupBy(a => a.Email);
            foreach (var group in studentGroups)
            {
                int present = group.Count(a => a.AttendanceStatus == "Present");
                int late = group.Count(a => a.AttendanceStatus == "Late");
                int total = group.Count();
                double rate = total > 0 ? ((present + (late * 0.5)) / total) * 100 : 0;

                if (rate < 75 && total >= 3) // Only show if they have at least 3 records
                {
                    var student = await _context.Spring2023_Group1_Profile_Sys
                        .FirstOrDefaultAsync(s => s.Email == group.Key);

                    StudentsWithPoorAttendance.Add(new StudentAttendanceSummary
                    {
                        StudentEmail = group.Key,
                        StudentName = student?.Name ?? "Unknown",
                        TotalSessions = total,
                        PresentCount = present,
                        AbsentCount = group.Count(a => a.AttendanceStatus == "Absent"),
                        LateCount = late,
                        AttendanceRate = rate
                    });
                }
            }

            // Calculate daily attendance trends
            var dailyGroups = attendanceRecords
                .GroupBy(a => a.CurrentDate.Date)
                .OrderBy(g => g.Key);

            foreach (var group in dailyGroups)
            {
                int present = group.Count(a => a.AttendanceStatus == "Present");
                int late = group.Count(a => a.AttendanceStatus == "Late");
                int total = group.Count();
                double rate = total > 0 ? ((present + (late * 0.5)) / total) * 100 : 0;

                AttendanceTrends.Add(new DailyAttendanceTrend
                {
                    Date = group.Key,
                    TotalRecords = total,
                    PresentCount = present,
                    AbsentCount = group.Count(a => a.AttendanceStatus == "Absent"),
                    AttendanceRate = rate
                });
            }

            // Sort collections
            ServiceStats = ServiceStats.OrderByDescending(s => s.AttendanceRate).ToList();
            SectionStats = SectionStats.OrderByDescending(s => s.AttendanceRate).ToList();
            StudentsWithPoorAttendance = StudentsWithPoorAttendance.OrderBy(s => s.AttendanceRate).ToList();
        }
    }

    public class ServiceAttendanceStats
    {
        public string ServiceName { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class SectionAttendanceStats
    {
        public int SectionID { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class StudentAttendanceSummary
    {
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class DailyAttendanceTrend
    {
        public DateTime Date { get; set; }
        public int TotalRecords { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendanceRate { get; set; }
    }
}