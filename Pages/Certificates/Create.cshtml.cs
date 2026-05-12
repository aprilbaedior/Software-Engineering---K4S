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

namespace CS395SI_Spring2023_Group1.Pages_Certificates
{
    [Authorize(Roles = "Manager,Admin")]
    public class CreateModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public CreateModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_Certificate Spring2026_Group1_Certificate { get; set; } = default!;

        // Dropdown data
        public List<ServiceOption> Services { get; set; } = new();
        public List<Spring2023_Group1_Profile_Sys> Students { get; set; } = new();

        public class ServiceOption
        {
            public string ServiceID { get; set; } = string.Empty;
            public string ServiceName { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load unique services from sections table
            Services = await _context.Spring2026_Group1_Sections!
                .Where(s => s.Status == "Active" || s.Status == "approved")
                .Select(s => new ServiceOption
                {
                    ServiceID = s.ServiceID ?? "",
                    ServiceName = s.ServiceName ?? ""
                })
                .Distinct()
                .OrderBy(s => s.ServiceName)
                .ToListAsync();

            // Load active students
            Students = await _context.Spring2023_Group1_Profile_Sys
                .Where(p => p.ApplicationStatus == "Approved")
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Page();
        }

        /// <summary>
        /// API endpoint to get sections for a specific service (for cascading dropdown)
        /// </summary>
        public async Task<IActionResult> OnGetSectionsAsync(string serviceId)
        {
            if (string.IsNullOrEmpty(serviceId))
            {
                return new JsonResult(new List<object>());
            }

            var sections = await _context.Spring2026_Group1_Sections!
                .Where(s => s.ServiceID == serviceId && 
                           (s.Status == "Active" || s.Status == "approved"))
                .OrderBy(s => s.WeekDay)
                .ThenBy(s => s.StartTime)
                .Select(s => new
                {
                    sectionId = s.SectionID,
                    display = $"Section {s.SectionID} - {s.WeekDay} {(s.StartTime.HasValue ? s.StartTime.Value.ToString(@"hh\:mm") : "")} - {(s.EndTime.HasValue ? s.EndTime.Value.ToString(@"hh\:mm") : "")}"
                })
                .ToListAsync();

            return new JsonResult(sections);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdown data
                await OnGetAsync();
                return Page();
            }

            // Get section details
            var section = await _context.Spring2026_Group1_Sections!
                .FirstOrDefaultAsync(s => s.SectionID == Spring2026_Group1_Certificate.SectionID);

            if (section == null)
            {
                ModelState.AddModelError("", "Selected section not found.");
                await OnGetAsync();
                return Page();
            }

            // Get student details
            var student = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(p => p.Email == Spring2026_Group1_Certificate.StudentEmail);

            if (student == null)
            {
                ModelState.AddModelError("", "Selected student not found.");
                await OnGetAsync();
                return Page();
            }

            // Generate new certificate ID
            var maxId = await _context.Spring2026_Group1_Certificate.AnyAsync()
                ? await _context.Spring2026_Group1_Certificate.MaxAsync(c => c.CertificateID)
                : 0;

            // Populate certificate
            Spring2026_Group1_Certificate.CertificateID = maxId + 1;
            Spring2026_Group1_Certificate.StudentName = student.Name;
            Spring2026_Group1_Certificate.ServiceID = section.ServiceID ?? "";
            Spring2026_Group1_Certificate.ServiceName = section.ServiceName ?? "";
            Spring2026_Group1_Certificate.IssuedDate = DateTime.UtcNow;
            Spring2026_Group1_Certificate.IssuedBy = User.Identity?.Name ?? "Unknown";

            _context.Spring2026_Group1_Certificate.Add(Spring2026_Group1_Certificate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Certificate issued to {student.Name} for {section.ServiceName}.";
            return RedirectToPage("./Index");
        }
    }
}
