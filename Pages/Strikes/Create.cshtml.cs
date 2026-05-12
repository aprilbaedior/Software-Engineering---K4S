using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS395SI_Spring2023_Group1.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages_Strikes
{
    [Authorize(Roles = MyConstants.ROLE_INSTRUCTOR)]
    public class CreateModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public CreateModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_Strike Spring2026_Group1_Strike { get; set; } = new();

        // View model for displaying student info
        public List<StudentDisplayInfo> Students { get; set; } = new();

        public class StudentDisplayInfo
        {
            public int StudentID { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string DisplayText => $"{Name} ({Email})";
        }

        public async Task OnGetAsync()
        {
            // Get instructor's email
            var instructorEmail = User?.Identity?.Name;

            if (string.IsNullOrEmpty(instructorEmail))
            {
                Students = new List<StudentDisplayInfo>();
                return;
            }

            // Get sections taught by this instructor
            var instructorSections = await _context.Spring2026_Group1_Sections!
                .Where(s => s.Instructor != null && s.Instructor.Email == instructorEmail)
                .Select(s => s.SectionID)
                .ToListAsync();

            if (!instructorSections.Any())
            {
                Students = new List<StudentDisplayInfo>();
                return;
            }

            // Get students enrolled in those sections with their profile information
            Students = await (
                from student in _context.Spring2026_Group1_Students
                join schedule in _context.Spring2024_Group2_Schedule
                    on student.Email equals schedule.StudentEmail
                join profile in _context.Spring2023_Group1_Profile_Sys
                    on student.Email equals profile.Email
                where instructorSections.Contains(schedule.SectionID)
                select new StudentDisplayInfo
                {
                    StudentID = student.StudentID,
                    Email = student.Email,
                    Name = profile.Name
                })
                .Distinct()
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Unable to determine current instructor.");
                await OnGetAsync();
                return Page();
            }

            // Only validate the fields that come from the form
            // Remove validation errors for fields we'll populate server-side
            ModelState.Remove("Spring2026_Group1_Strike.FiledBy");
            ModelState.Remove("Spring2026_Group1_Strike.StudentEmail");
            ModelState.Remove("Spring2026_Group1_Strike.StudentName");
            ModelState.Remove("Spring2026_Group1_Strike.FiledDate");
            ModelState.Remove("Spring2026_Group1_Strike.ReviewStatus");

            // Validate the user-provided fields
            if (Spring2026_Group1_Strike.StudentID <= 0)
            {
                ModelState.AddModelError("Spring2026_Group1_Strike.StudentID", "Please select a student.");
            }

            if (string.IsNullOrWhiteSpace(Spring2026_Group1_Strike.Reason))
            {
                ModelState.AddModelError("Spring2026_Group1_Strike.Reason", "Please provide a reason for the strike.");
            }

            // Check if there are any validation errors
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Get the instructor's InstructorID
            var instructor = await _context.Spring2026_Group1_Instructor!
                .FirstOrDefaultAsync(i => i.Email == email);

            if (instructor == null)
            {
                ModelState.AddModelError(string.Empty, "Instructor profile not found. Please contact support.");
                await OnGetAsync();
                return Page();
            }

            // Get student details
            var student = await _context.Spring2026_Group1_Students
                .FirstOrDefaultAsync(s => s.StudentID == Spring2026_Group1_Strike.StudentID);
            
            if (student == null)
            {
                ModelState.AddModelError(string.Empty, "Selected student not found.");
                await OnGetAsync();
                return Page();
            }

            var profile = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(p => p.Email == student.Email);

            // Generate next StrikeID
            var maxStrikeId = await _context.Spring2026_Group1_Strike.AnyAsync()
                ? await _context.Spring2026_Group1_Strike.MaxAsync(s => s.StrikeID)
                : 0;

            // Set all required fields including StrikeID and InstructorID
            Spring2026_Group1_Strike.StrikeID = maxStrikeId + 1;
            Spring2026_Group1_Strike.InstructorID = instructor.InstructorID;
            Spring2026_Group1_Strike.StudentEmail = student.Email;
            Spring2026_Group1_Strike.StudentName = profile?.Name ?? student.Email;
            Spring2026_Group1_Strike.FiledDate = DateTime.UtcNow;
            Spring2026_Group1_Strike.FiledBy = email;
            Spring2026_Group1_Strike.ReviewStatus = "Pending";
            Spring2026_Group1_Strike.ReviewedBy = null;
            Spring2026_Group1_Strike.ReviewDate = null;
            Spring2026_Group1_Strike.ManagerNotes = null;
            Spring2026_Group1_Strike.ManagerID = null;

            // Try to get section context
            var instructorSection = await _context.Spring2024_Group2_Schedule
                .Where(s => s.StudentEmail == student.Email)
                .Join(_context.Spring2026_Group1_Sections!,
                    schedule => schedule.SectionID,
                    section => section.SectionID,
                    (schedule, section) => new { schedule, section })
                .Where(x => x.section.Instructor != null && x.section.Instructor.Email == email)
                .Select(x => new { x.schedule.SectionID, x.schedule.ServiceID })
                .FirstOrDefaultAsync();

            if (instructorSection != null)
            {
                Spring2026_Group1_Strike.SectionID = instructorSection.SectionID;
                Spring2026_Group1_Strike.ServiceID = instructorSection.ServiceID;
            }

            try
            {
                _context.Spring2026_Group1_Strike.Add(Spring2026_Group1_Strike);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Strike filed successfully against {Spring2026_Group1_Strike.StudentName}!";

                // Clear the Strike object to prevent resubmission
                Spring2026_Group1_Strike = new Spring2026_Group1_Strike();

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                // Build detailed error message including inner exceptions
                var errorMessage = ex.Message;
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    errorMessage += $" | Inner: {innerEx.Message}";
                    innerEx = innerEx.InnerException;
                }
                
                ModelState.AddModelError(string.Empty, $"Error saving strike: {errorMessage}");
                await OnGetAsync();
                return Page();
            }
        }
    }
}
