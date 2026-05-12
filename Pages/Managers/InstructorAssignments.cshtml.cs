using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Manager
{
    [Authorize(Roles = "Manager,Admin")]
    public class InstructorAssignmentsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public InstructorAssignmentsModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2026_Group1_Instructor> Instructors { get; set; } = new();
        public List<Spring2026_Group1_Sections> Sections { get; set; } = new();
        public List<Spring2026_Group1_InstructorAssignment> Assignments { get; set; } = new();

        public async Task OnGetAsync()
        {
            Instructors = await _context.Spring2026_Group1_Instructor!
                .Where(i => i.Status == "Active")
                .OrderBy(i => i.FullName)
                .ToListAsync();

            Sections = await _context.Spring2026_Group1_Sections!
                .Where(s => s.Status == "Active" || s.Status == "approved" || s.Status == "pending")
                .OrderBy(s => s.ServiceName)
                .ThenBy(s => s.WeekDay)
                .ToListAsync();

            Assignments = await _context.Spring2026_Group1_InstructorAssignment!
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAssignAsync(int sectionId, string instructorEmail)
        {
            var instructor = await _context.Spring2026_Group1_Instructor!
                .FirstOrDefaultAsync(i => i.Email == instructorEmail);

            if (instructor == null)
            {
                TempData["ErrorMessage"] = "Instructor not found.";
                return RedirectToPage();
            }

            var section = await _context.Spring2026_Group1_Sections!.FindAsync(sectionId);
            if (section == null)
            {
                TempData["ErrorMessage"] = "Section not found.";
                return RedirectToPage();
            }

            // Update the section's InstructorID and mark as modified
            section.InstructorID = instructor.InstructorID;
            _context.Entry(section).State = EntityState.Modified;

            // Check if this section already has an assignment
            var existing = await _context.Spring2026_Group1_InstructorAssignment!
                .FirstOrDefaultAsync(a => a.SectionID == sectionId);

            if (existing != null)
            {
                // Replace existing assignment
                existing.InstructorEmail = instructorEmail;
                existing.InstructorName = instructor.FullName;
                existing.AssignedBy = User.Identity?.Name ?? "Unknown";
                existing.AssignedDate = DateTime.UtcNow;
                existing.ServiceName = section.ServiceName;
                _context.Entry(existing).State = EntityState.Modified;
            }
            else
            {
                var assignment = new Spring2026_Group1_InstructorAssignment
                {
                    InstructorEmail = instructorEmail,
                    InstructorName = instructor.FullName,
                    SectionID = sectionId,
                    ServiceName = section.ServiceName,
                    AssignedBy = User.Identity?.Name ?? "Unknown",
                    AssignedDate = DateTime.UtcNow
                };
                _context.Spring2026_Group1_InstructorAssignment!.Add(assignment);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{instructor.FullName} has been assigned to Section #{sectionId}.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int assignmentId)
        {
            var assignment = await _context.Spring2026_Group1_InstructorAssignment!.FindAsync(assignmentId);
            if (assignment != null)
            {
                // Also clear the InstructorID from the section
                var section = await _context.Spring2026_Group1_Sections!.FindAsync(assignment.SectionID);
                if (section != null)
                {
                    section.InstructorID = null;
                    _context.Entry(section).State = EntityState.Modified;
                }

                _context.Spring2026_Group1_InstructorAssignment!.Remove(assignment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor assignment removed.";
            }
            return RedirectToPage();
        }
    }
}