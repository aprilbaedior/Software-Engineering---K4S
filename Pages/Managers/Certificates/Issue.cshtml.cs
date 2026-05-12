using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CS395SI_Spring2023_Group1.Pages.Manager.Certificates
{
    [Authorize(Roles = "Manager,Admin")]
    public class IssueModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IssueModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        // Dropdowns for manual form (Students list no longer needed for display)
        public List<ServiceOption> Services { get; set; } = new();

        public class ServiceOption
        {
            public string ServiceID { get; set; } = string.Empty;
            public string ServiceName { get; set; } = string.Empty;
        }

        // Eligibility check
        [BindProperty(SupportsGet = true)]
        public string? SelectedServiceId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? SelectedSectionId { get; set; }
        
        public Spring2026_Group1_Sections? SelectedSection { get; set; }
        public List<StudentEligibility> EligibilityList { get; set; } = new();

        public class StudentEligibility
        {
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public double AttendancePercent { get; set; }
            public int TotalMarked { get; set; }
            public int ApprovedStrikes { get; set; }
            public bool AlreadyCertified { get; set; }
            public bool MeetsAttendance => AttendancePercent >= 75.0;
            public bool MeetsStrikeLimit => ApprovedStrikes <= 3;
            public bool IsEligible => MeetsAttendance && MeetsStrikeLimit && !AlreadyCertified;
        }

        public async Task OnGetAsync()
        {
            await LoadDropdownsAsync();

            if (SelectedSectionId.HasValue)
            {
                SelectedSection = await _context.Spring2026_Group1_Sections!
                    .FirstOrDefaultAsync(s => s.SectionID == SelectedSectionId.Value);

                if (SelectedSection != null)
                {
                    SelectedServiceId = SelectedSection.ServiceID;
                    await LoadEligibilityAsync(SelectedSectionId.Value, SelectedSection.ServiceID, SelectedSection.ServiceName);
                }
            }
        }

        /// <summary>
        /// API endpoint to search for students by name or email
        /// </summary>
        public async Task<IActionResult> OnGetSearchStudentsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2 || query.Length > 100)
            {
                return new JsonResult(new List<object>());
            }

            var searchTerm = query.Trim().ToLower();

            var students = await _context.Spring2023_Group1_Profile_Sys
                .Where(s => s.ApplicationStatus == "Approved" && 
                           (s.Name.ToLower().Contains(searchTerm) || 
                            s.Email.ToLower().Contains(searchTerm)))
                .OrderBy(s => s.Name)
                .Take(10)
                .Select(s => new
                {
                    email = s.Email,
                    name = s.Name ?? s.Email
                })
                .ToListAsync();

            return new JsonResult(students);
        }

        /// <summary>
        /// API endpoint to get sections for a specific service (for cascading dropdown)
        /// </summary>
        public async Task<IActionResult> OnGetSectionsForServiceAsync(string serviceId)
        {
            if (string.IsNullOrEmpty(serviceId))
            {
                return new JsonResult(new List<object>());
            }

            var sections = await _context.Spring2026_Group1_Sections!
                .Where(s => s.ServiceID == serviceId && (s.Status == "Active" || s.Status == "approved" || s.Status == "pending"))
                .OrderBy(s => s.WeekDay)
                .ThenBy(s => s.StartTime)
                .Select(s => new
                {
                    sectionId = s.SectionID,
                    display = $"Section #{s.SectionID} — {s.WeekDay} {(s.StartTime.HasValue ? s.StartTime.Value.ToString(@"hh\:mm") : "")} [{s.Status}]"
                })
                .ToListAsync();

            return new JsonResult(sections);
        }

        public async Task<IActionResult> OnPostAsync(
            string studentEmail, string studentName,
            string serviceId, string serviceName,
            int sectionId, string? notes)
        {
            if (string.IsNullOrEmpty(studentEmail) || string.IsNullOrEmpty(serviceId))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                await LoadDropdownsAsync();
                return Page();
            }

            var section = await _context.Spring2026_Group1_Sections!
                .FirstOrDefaultAsync(s => s.SectionID == sectionId);
            
            if (section == null)
            {
                TempData["ErrorMessage"] = "Invalid section selected.";
                await LoadDropdownsAsync();
                return Page();
            }

            var student = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(s => s.Email == studentEmail && s.ApplicationStatus == "Approved");
            
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found or not approved.";
                await LoadDropdownsAsync();
                return Page();
            }

            var existingCert = await _context.Spring2026_Group1_Certificate!
                .FirstOrDefaultAsync(c => c.StudentEmail == studentEmail && c.SectionID == sectionId);
            
            if (existingCert != null)
            {
                TempData["ErrorMessage"] = "Certificate already issued to this student for this section.";
                await LoadDropdownsAsync();
                return Page();
            }

            int nextCertId = (_context.Spring2026_Group1_Certificate!.Any()
                ? await _context.Spring2026_Group1_Certificate!.MaxAsync(c => c.CertificateID)
                : 0) + 1;

            var certificate = new Spring2026_Group1_Certificate
            {
                CertificateID = nextCertId,
                StudentEmail = studentEmail,
                StudentName = studentName ?? studentEmail,
                StudentID = studentEmail,
                ServiceID = serviceId,
                ServiceName = serviceName ?? serviceId,
                SectionID = sectionId,
                IssuedDate = DateTime.UtcNow,
                IssuedBy = User.Identity?.Name ?? "Unknown",
                ManagerID = User.Identity?.Name ?? "Unknown",
                CertificateStatus = "Issued",
                Notes = notes
            };

            _context.Spring2026_Group1_Certificate!.Add(certificate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Certificate successfully issued to {studentName} for {serviceName}.";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostQuickIssueAsync(
            string studentEmail, string studentName,
            string serviceId, string serviceName,
            int sectionId)
        {
            var section = await _context.Spring2026_Group1_Sections!
                .FirstOrDefaultAsync(s => s.SectionID == sectionId);
            
            if (section == null)
            {
                TempData["ErrorMessage"] = "Invalid section.";
                return RedirectToPage(new { selectedServiceId = serviceId, selectedSectionId = sectionId });
            }

            var existingCert = await _context.Spring2026_Group1_Certificate!
                .FirstOrDefaultAsync(c => c.StudentEmail == studentEmail && c.SectionID == sectionId);
            
            if (existingCert != null)
            {
                TempData["ErrorMessage"] = "Certificate already issued to this student.";
                return RedirectToPage(new { selectedServiceId = serviceId, selectedSectionId = sectionId });
            }

            int nextCertId = (_context.Spring2026_Group1_Certificate!.Any()
                ? await _context.Spring2026_Group1_Certificate!.MaxAsync(c => c.CertificateID)
                : 0) + 1;

            var certificate = new Spring2026_Group1_Certificate
            {
                CertificateID = nextCertId,
                StudentEmail = studentEmail,
                StudentName = studentName,
                StudentID = studentEmail,
                ServiceID = serviceId,
                ServiceName = serviceName,
                SectionID = sectionId,
                IssuedDate = DateTime.UtcNow,
                IssuedBy = User.Identity?.Name ?? "Unknown",
                ManagerID = User.Identity?.Name ?? "Unknown",
                CertificateStatus = "Issued",
                Notes = "Auto-issued based on attendance and strike eligibility."
            };

            _context.Spring2026_Group1_Certificate!.Add(certificate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Certificate issued to {studentName}.";
            return RedirectToPage(new { selectedServiceId = serviceId, selectedSectionId = sectionId });
        }

        private async Task LoadDropdownsAsync()
        {
            var allSections = await _context.Spring2026_Group1_Sections!
                .Where(s => !string.IsNullOrEmpty(s.ServiceID) && 
                           !string.IsNullOrEmpty(s.ServiceName) &&
                           (s.Status == "Active" || s.Status == "approved" || s.Status == "pending"))
                .Select(s => new { s.ServiceID, s.ServiceName })
                .ToListAsync();

            Services = allSections
                .GroupBy(s => new { s.ServiceID, s.ServiceName })
                .Select(g => new ServiceOption
                {
                    ServiceID = g.Key.ServiceID!,
                    ServiceName = g.Key.ServiceName!
                })
                .OrderBy(s => s.ServiceName)
                .ToList();
        }

        private async Task LoadEligibilityAsync(int sectionId, string? serviceId, string? serviceName)
        {
            var enrolled = await (
                from sched in _context.Spring2024_Group2_Schedule
                join profile in _context.Spring2023_Group1_Profile_Sys
                    on sched.StudentEmail equals profile.Email
                where sched.SectionID == sectionId
                select new { sched.StudentEmail, profile.Name }
            ).Distinct().ToListAsync();

            var attendanceRecords = await _context.Spring2025_Group3_Attendance!
                .Where(a => a.SectionID == sectionId && a.AttendanceStatus != "Not Marked")
                .ToListAsync();

            var strikes = await _context.Spring2026_Group1_Strike
                .Where(s => s.SectionID == sectionId && s.ReviewStatus == "Approved")
                .ToListAsync();

            var certified = await _context.Spring2026_Group1_Certificate!
                .Where(c => c.SectionID == sectionId)
                .Select(c => c.StudentEmail)
                .ToListAsync();

            foreach (var student in enrolled)
            {
                var studentRecords = attendanceRecords
                    .Where(a => a.Email == student.StudentEmail).ToList();

                int totalMarked = studentRecords.Count;
                double attendancePct = 0;

                if (totalMarked > 0)
                {
                    double weighted =
                        studentRecords.Count(a => a.AttendanceStatus == "Present") * 1.0 +
                        studentRecords.Count(a => a.AttendanceStatus == "Late") * 0.8 +
                        studentRecords.Count(a => a.AttendanceStatus == "Excused") * 0.5;

                    attendancePct = (weighted / totalMarked) * 100.0;
                }

                int approvedStrikes = strikes.Count(s => s.StudentEmail == student.StudentEmail);
                bool alreadyCertified = certified.Contains(student.StudentEmail);

                EligibilityList.Add(new StudentEligibility
                {
                    Email = student.StudentEmail,
                    Name = student.Name ?? student.StudentEmail,
                    AttendancePercent = Math.Round(attendancePct, 1),
                    TotalMarked = totalMarked,
                    ApprovedStrikes = approvedStrikes,
                    AlreadyCertified = alreadyCertified
                });
            }

            EligibilityList = EligibilityList
                .OrderByDescending(e => e.IsEligible)
                .ThenBy(e => e.Name)
                .ToList();
        }
    }
}
