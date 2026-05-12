using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Sections
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
        public Spring2026_Group1_Sections Spring2026_Group1_Sections { get; set; } = new Spring2026_Group1_Sections();

        [BindProperty]
        public List<string> SelectedDays { get; set; } = new();

        public string ServiceID { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public SelectList InstructorList { get; set; } = default!;

        public IActionResult OnGet(string serviceID)
        {
            if (string.IsNullOrEmpty(serviceID))
            {
                return RedirectToPage("/ServicesForAdmin/Index");
            }

            // Trim the serviceID in case it has padding
            var trimmedServiceID = serviceID?.Trim() ?? string.Empty;

            // Retrieve the service from the database
            var service = _context.Spring2023_Group1_Services
                .Where(s => s.ServiceID.Trim() == trimmedServiceID)
                .FirstOrDefault();

            if (service == null)
            {
                return RedirectToPage("/ServicesForAdmin/Index");
            }

            ServiceID = service.ServiceID;
            ServiceName = service.ServiceName ?? "Unknown Service";

            // Populate instructor dropdown with FullName as display text
            InstructorList = new SelectList(
                _context.Spring2026_Group1_Instructor
                    .Where(i => i.IsActive == true || i.IsActive == null)
                    .OrderBy(i => i.FullName),
                "InstructorID",
                "FullName"
            );

            // Pre-fill ServiceName and ServiceID in the model
            Spring2026_Group1_Sections.ServiceName = ServiceName;
            Spring2026_Group1_Sections.ServiceID = service.ServiceID;
            // Set default capacity
            Spring2026_Group1_Sections.Capacity = 20;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string serviceID)
        {
            // Get service from database to ensure we have the exact ServiceID format
            var trimmedServiceID = serviceID?.Trim() ?? string.Empty;
            var service = await _context.Spring2023_Group1_Services
                .Where(s => s.ServiceID.Trim() == trimmedServiceID)
                .FirstOrDefaultAsync();

            if (service == null)
            {
                ModelState.AddModelError("", "Invalid Service ID");
                ServiceID = serviceID;
                ServiceName = "Unknown Service";
                InstructorList = new SelectList(
                    _context.Spring2026_Group1_Instructor
                        .Where(i => i.IsActive == true || i.IsActive == null)
                        .OrderBy(i => i.FullName),
                    "InstructorID",
                    "FullName"
                );
                return Page();
            }

            // Use the exact ServiceID from the database (with proper padding)
            Spring2026_Group1_Sections.ServiceID = service.ServiceID;
            Spring2026_Group1_Sections.ServiceName = service.ServiceName ?? string.Empty;

            // Join selected days into a comma-separated string
            Spring2026_Group1_Sections.WeekDay = SelectedDays.Any()
                ? string.Join(",", SelectedDays)
                : null;

            // Ensure Capacity has a valid value (between 1 and 100 as a safe range)
            if (Spring2026_Group1_Sections.Capacity <= 0 || Spring2026_Group1_Sections.Capacity > 100)
            {
                ModelState.AddModelError("Spring2026_Group1_Sections.Capacity", "Capacity must be between 1 and 100");
                ServiceID = service.ServiceID;
                ServiceName = service.ServiceName ?? "Unknown Service";
                InstructorList = new SelectList(
                    _context.Spring2026_Group1_Instructor
                        .Where(i => i.IsActive == true || i.IsActive == null)
                        .OrderBy(i => i.FullName),
                    "InstructorID",
                    "FullName"
                );
                return Page();
            }

            // Generate the next SectionID manually (auto-increment)
            var maxSectionID = await _context.Spring2026_Group1_Sections
                .MaxAsync(s => (int?)s.SectionID) ?? 0;
            Spring2026_Group1_Sections.SectionID = maxSectionID + 1;

            // Handle empty instructor selection (convert 0 to null)
            if (Spring2026_Group1_Sections.InstructorID == 0)
            {
                Spring2026_Group1_Sections.InstructorID = null;
            }

            // Remove InstructorID validation errors if it's null
            ModelState.Remove("Spring2026_Group1_Sections.InstructorID");
            ModelState.Remove("Spring2026_Group1_Sections.SectionID");
            ModelState.Remove("Spring2026_Group1_Sections.ServiceID");
            ModelState.Remove("Spring2026_Group1_Sections.ServiceName");
            ModelState.Remove("Spring2026_Group1_Sections.Instructor");

            if (!ModelState.IsValid)
            {
                // Repopulate dropdown on error
                ServiceID = service.ServiceID;
                ServiceName = service.ServiceName ?? "Unknown Service";
                InstructorList = new SelectList(
                    _context.Spring2026_Group1_Instructor
                        .Where(i => i.IsActive == true || i.IsActive == null)
                        .OrderBy(i => i.FullName),
                    "InstructorID",
                    "FullName"
                );
                return Page();
            }

            try
            {
                _context.Spring2026_Group1_Sections.Add(Spring2026_Group1_Sections);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new { serviceID = service.ServiceID });
            }
            catch (DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Error: {ex.InnerException?.Message}");
                ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");

                ServiceID = service.ServiceID;
                ServiceName = service.ServiceName ?? "Unknown Service";
                InstructorList = new SelectList(
                    _context.Spring2026_Group1_Instructor
                        .Where(i => i.IsActive == true || i.IsActive == null)
                        .OrderBy(i => i.FullName),
                    "InstructorID",
                    "FullName"
                );
                return Page();
            }
        }
    }
}
