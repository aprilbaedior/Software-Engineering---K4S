using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace CS395SI_Spring2023_Group1.Pages.SectionForStudent
{
    public class SectionEnrollModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SectionEnrollModel> _logger;

        public SectionEnrollModel(
            CS395SI_Spring2023_Group1Context context,
            IConfiguration configuration,
            ILogger<SectionEnrollModel> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // Updated to match DbContext and Razor page
        public IList<Spring2026_Group1_Sections> Spring2026_Group1_Sections { get; set; } = new List<Spring2026_Group1_Sections>();

        // Bind POST inputs
        [BindProperty]
        public string? ServiceID { get; set; }

        [BindProperty]
        public string? ServiceName { get; set; }

        [BindProperty]
        public int SectionID { get; set; }

        [BindProperty]
        public string? StartDateString { get; set; }

        [BindProperty]
        public string? EndDateString { get; set; }

        [BindProperty]
        public string? WeekDay { get; set; }

        [BindProperty]
        public string? StartTimeString { get; set; }

        [BindProperty]
        public string? EndTimeString { get; set; }

        public async Task<IActionResult> OnGetAsync(string ServiceID)
        {
            string? studentEmail = HttpContext.Session.GetString("studentEmail")
                ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(studentEmail))
            {
                return RedirectToPage("/Account/Login");
            }
            HttpContext.Session.SetString("studentEmail", studentEmail);

            if (_context.Spring2026_Group1_Sections != null)
            {
                // Only show approved sections to students
                Spring2026_Group1_Sections = await _context.Spring2026_Group1_Sections
                    .Where(s => s.ServiceID == ServiceID && s.Status == "Active")
                    .ToListAsync();
            }

            if (_context.Spring2026_Group1_Schedule != null)
            {
                var enrolledSections = await _context.Spring2026_Group1_Schedule
                    .Where(s => s.StudentEmail == studentEmail)
                    .Select(s => s.SectionID)
                    .ToListAsync();

                ViewData["EnrolledSections"] = enrolledSections;
            }

            // Check if user is approved
            bool isApproved = false;
            try
            {
                var profile = await _context.Spring2023_Group1_Profile_Sys
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Email == studentEmail);

                if (profile != null)
                {
                    isApproved = string.Equals(profile.ApplicationStatus, "Approved", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking approval status for {Email}", studentEmail);
                isApproved = true;
            }

            ViewData["IsUserApproved"] = isApproved;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string? studentEmail = HttpContext.Session.GetString("studentEmail")
                ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(studentEmail))
            {
                return new JsonResult(new { success = false, message = "User not authenticated." });
            }
            HttpContext.Session.SetString("studentEmail", studentEmail);

            // Check if user is approved
            bool isApproved;
            try
            {
                isApproved = await IsUserApprovedAsync(studentEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying approval status for {Email}", studentEmail);
                return new JsonResult(new { success = false, message = "Error verifying approval status." });
            }

            if (!isApproved)
            {
                return new JsonResult(new { success = false, message = "Your application has not been approved yet. You cannot enroll in courses at this time." });
            }

            // Validate SectionID
            if (SectionID <= 0)
            {
                return new JsonResult(new { success = false, message = "Invalid section ID." });
            }

            if (_context.Spring2026_Group1_Schedule != null)
            {
                bool alreadyEnrolled = await _context.Spring2026_Group1_Schedule
                    .AnyAsync(s => s.StudentEmail == studentEmail && s.SectionID == SectionID);

                if (alreadyEnrolled)
                {
                    return new JsonResult(new { success = false, message = "You are already enrolled in this course." });
                }
            }

            // Parse dates and times from strings
            DateTime? startDate = null;
            DateTime? endDate = null;
            TimeSpan? startTime = null;
            TimeSpan? endTime = null;

            if (!string.IsNullOrEmpty(StartDateString) && DateTime.TryParse(StartDateString, out DateTime parsedStartDate))
            {
                startDate = parsedStartDate;
            }

            if (!string.IsNullOrEmpty(EndDateString) && DateTime.TryParse(EndDateString, out DateTime parsedEndDate))
            {
                endDate = parsedEndDate;
            }

            if (!string.IsNullOrEmpty(StartTimeString) && TimeSpan.TryParse(StartTimeString, out TimeSpan parsedStartTime))
            {
                startTime = parsedStartTime;
            }

            if (!string.IsNullOrEmpty(EndTimeString) && TimeSpan.TryParse(EndTimeString, out TimeSpan parsedEndTime))
            {
                endTime = parsedEndTime;
            }

            var groupSchedule = new Spring2026_Group1_Schedule
            {
                ServiceID = ServiceID ?? string.Empty,
                ServiceName = ServiceName,
                SectionID = SectionID,
                StartDate = startDate ?? default, // Fix: use default if null
                EndDate = endDate ?? default,     // Fix: use default if null
                WeekDay = WeekDay ?? string.Empty,
                StartTime = startTime ?? default, // Fix: use default if null
                EndTime = endTime ?? default,
                StudentEmail = studentEmail,
                EnrolledDate = DateTime.UtcNow
            };

            try
            {
                if (_context.Spring2026_Group1_Schedule != null)
                {
                    _context.Spring2026_Group1_Schedule.Add(groupSchedule);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving schedule for {Email}, Section {SectionID}", studentEmail, SectionID);
                // Return more detailed error for debugging
                return new JsonResult(new { 
                    success = false, 
                    message = $"Error saving enrollment: {ex.InnerException?.Message ?? ex.Message}" 
                });
            }

            return new JsonResult(new { success = true, sectionID = SectionID });
        }

        private async Task<bool> IsUserApprovedAsync(string? studentEmail)
        {
            if (string.IsNullOrEmpty(studentEmail))
                return false;

            var profile = await _context.Spring2023_Group1_Profile_Sys
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Email == studentEmail);

            return profile != null && string.Equals(profile.ApplicationStatus, "Approved", StringComparison.OrdinalIgnoreCase);
        }
    }
}