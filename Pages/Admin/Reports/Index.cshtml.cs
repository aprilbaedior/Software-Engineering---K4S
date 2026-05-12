using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Admin.Reports
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(
            CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Overview Statistics
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalServices { get; set; }
        public int TotalSections { get; set; }
        public int ActiveEnrollments { get; set; }

        // Enrollment Trends
        public List<EnrollmentTrend> EnrollmentTrends { get; set; } = new List<EnrollmentTrend>();
        public List<ServiceEnrollment> ServiceEnrollments { get; set; } = new List<ServiceEnrollment>();

        // Application Statistics
        public int PendingStudentApplications { get; set; }
        public int ApprovedStudentApplications { get; set; }
        public int DeniedStudentApplications { get; set; }
        public int PendingInstructorApplications { get; set; }
        public int ApprovedInstructorApplications { get; set; }

        // Conduct & Certificates
        public int TotalStrikes { get; set; }
        public int ActiveStrikes { get; set; }
        public int ResolvedStrikes { get; set; }
        public int TotalCertificates { get; set; }

        // Recent Activity
        public List<RecentActivity> RecentActivities { get; set; } = new List<RecentActivity>();

        public async Task OnGetAsync()
        {
            // User Statistics
            var allUsers = await _userManager.Users.ToListAsync();
            TotalUsers = allUsers.Count;

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Student")) TotalStudents++;
                if (roles.Contains("Instructor")) TotalInstructors++;
            }

            // Service & Section Statistics
            TotalServices = await _context.Spring2023_Group1_Services.CountAsync();
            TotalSections = await _context.Spring2026_Group1_Sections.CountAsync();
            ActiveEnrollments = await _context.Spring2026_Group1_Schedule.CountAsync();

            // Application Statistics
            var studentProfiles = await _context.Spring2023_Group1_Profile_Sys.ToListAsync();
            PendingStudentApplications = studentProfiles.Count(p => p.ApplicationStatus == "Pending");
            ApprovedStudentApplications = studentProfiles.Count(p => p.ApplicationStatus == "Approved");
            DeniedStudentApplications = studentProfiles.Count(p => p.ApplicationStatus == "Denied");

            var instructorApps = await _context.Spring2026_Group1_EmployeeApplication.ToListAsync();
            PendingInstructorApplications = instructorApps.Count(a => a.ApplicationStatus == "Pending");
            ApprovedInstructorApplications = instructorApps.Count(a => a.ApplicationStatus == "Approved");

            // Conduct Statistics - FIXED: ReviewStatus instead of Status
            var strikes = await _context.Spring2026_Group1_Strike.ToListAsync();
            TotalStrikes = strikes.Count;
            ActiveStrikes = strikes.Count(s => s.ReviewStatus == "Pending" || s.ReviewStatus == "Approved");
            ResolvedStrikes = strikes.Count(s => s.ReviewStatus == "Denied");

            TotalCertificates = await _context.Spring2026_Group1_Certificate.CountAsync();

            // Enrollment Trends (last 30 days) - FIXED: EnrolledDate instead of EnrollmentDate
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var schedules = await _context.Spring2026_Group1_Schedule
                .Where(s => s.EnrolledDate >= thirtyDaysAgo)
                .ToListAsync();

            var groupedByDate = schedules
                .GroupBy(s => s.EnrolledDate.Date)
                .OrderBy(g => g.Key);

            foreach (var group in groupedByDate)
            {
                EnrollmentTrends.Add(new EnrollmentTrend
                {
                    Date = group.Key,
                    Count = group.Count()
                });
            }

            // Service Enrollment Breakdown
            var services = await _context.Spring2023_Group1_Services.ToListAsync();
            foreach (var service in services)
            {
                var enrollmentCount = await _context.Spring2026_Group1_Schedule
                    .CountAsync(s => s.ServiceID == service.ServiceID);

                var sectionCount = await _context.Spring2026_Group1_Sections
                    .CountAsync(sec => sec.ServiceID == service.ServiceID);

                ServiceEnrollments.Add(new ServiceEnrollment
                {
                    ServiceName = service.ServiceName,
                    EnrollmentCount = enrollmentCount,
                    SectionCount = sectionCount,
                    AveragePerSection = sectionCount > 0 ? (double)enrollmentCount / sectionCount : 0
                });
            }

            ServiceEnrollments = ServiceEnrollments.OrderByDescending(s => s.EnrollmentCount).ToList();

            // Recent Activity (last 10 items)
            var recentSchedules = await _context.Spring2026_Group1_Schedule
                .OrderByDescending(s => s.EnrolledDate)
                .Take(5)
                .ToListAsync();

            foreach (var sched in recentSchedules)
            {
                var student = await _context.Spring2023_Group1_Profile_Sys
                    .FirstOrDefaultAsync(p => p.Email == sched.StudentEmail);

                var service = await _context.Spring2023_Group1_Services
                    .FirstOrDefaultAsync(s => s.ServiceID == sched.ServiceID);

                RecentActivities.Add(new RecentActivity
                {
                    Type = "Enrollment",
                    Description = $"{student?.Name ?? "Student"} enrolled in {service?.ServiceName ?? "service"}",
                    Timestamp = sched.EnrolledDate,
                    Icon = "person-plus",
                    BadgeClass = "bg-success"
                });
            }

            // FIXED: FiledDate instead of DateIssued
            var recentStrikes = await _context.Spring2026_Group1_Strike
                .OrderByDescending(s => s.FiledDate)
                .Take(5)
                .ToListAsync();

            foreach (var strike in recentStrikes)
            {
                RecentActivities.Add(new RecentActivity
                {
                    Type = "Strike",
                    Description = $"Strike issued to {strike.StudentEmail}: {strike.Reason?.Substring(0, Math.Min(50, strike.Reason?.Length ?? 0))}...",
                    Timestamp = strike.FiledDate,
                    Icon = "flag-fill",
                    BadgeClass = "bg-danger"
                });
            }

            RecentActivities = RecentActivities.OrderByDescending(a => a.Timestamp).Take(10).ToList();
        }
    }

    public class EnrollmentTrend
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class ServiceEnrollment
    {
        public string ServiceName { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
        public int SectionCount { get; set; }
        public double AveragePerSection { get; set; }
    }

    public class RecentActivity
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string BadgeClass { get; set; } = string.Empty;
    }
}