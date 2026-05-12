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

namespace CS395SI_Spring2026_Group1.Pages_Instructors
{
    [Authorize(Roles = "Instructor,Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context) => _context = context;

        [BindProperty(SupportsGet = true)]
        public int? SectionID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public class ContactRow
        {
            public int StudentId { get; set; }
            public string StudentName { get; set; } = string.Empty;
            public string? EmerName { get; set; }
            public string? EmerRelation { get; set; }
            public string? EmerPhone1 { get; set; }
            public string? EmerName2 { get; set; }
            public string? EmerRelation2 { get; set; }
            public string? EmerPhone2 { get; set; }
            public string? EmerName3 { get; set; }
            public string? EmerRelation3 { get; set; }
            public string? EmerPhone3 { get; set; }
        }

        public List<ContactRow> ContactRows { get; set; } = new();

        // Pagination properties
        public int PageSize { get; set; } = 15;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public async Task OnGetAsync()
        {
            PageNumber = PageNumber > 0 ? PageNumber : 1;

            var userEmail = User.Identity?.Name;
            List<int> sectionIds = new();

            // If a specific section is requested, use that
            if (SectionID.HasValue)
            {
                // Verify the instructor has access to this section
                var hasAccess = await _context.Spring2026_Group1_InstructorAssignment
                    .AsNoTracking()
                    .AnyAsync(a => a.SectionID == SectionID.Value && a.InstructorEmail == userEmail);

                if (hasAccess)
                {
                    sectionIds.Add(SectionID.Value);
                }
            }
            else
            {
                // Load all sections for this instructor
                if (!string.IsNullOrEmpty(userEmail))
                {
                    sectionIds = await _context.Spring2026_Group1_InstructorAssignment
                        .AsNoTracking()
                        .Where(a => a.InstructorEmail == userEmail)
                        .Select(a => a.SectionID)
                        .Distinct()
                        .ToListAsync();
                }
            }

            // If no sections found, return empty list
            if (sectionIds == null || sectionIds.Count == 0)
            {
                ContactRows = new List<ContactRow>();
                ViewData["ContactRows"] = ContactRows;
                TotalRecords = 0;
                TotalPages = 0;
                return;
            }

            // Find student emails from schedules in those sections
            var studentEmails = await _context.Spring2026_Group1_Schedule
                .AsNoTracking()
                .Where(s => sectionIds.Contains(s.SectionID))
                .Select(s => s.StudentEmail)
                .Where(e => e != null)
                .Distinct()
                .ToListAsync();

            if (studentEmails == null || studentEmails.Count == 0)
            {
                ContactRows = new List<ContactRow>();
                ViewData["ContactRows"] = ContactRows;
                TotalRecords = 0;
                TotalPages = 0;
                return;
            }

            // Build query for contact rows
            var query = _context.Spring2023_Group1_Profile_Sys
                .AsNoTracking()
                .Where(s => studentEmails.Contains(s.Email));

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(searchLower) ||
                    s.Email.ToLower().Contains(searchLower) ||
                    (s.EmerName != null && s.EmerName.ToLower().Contains(searchLower)) ||
                    (s.EmerName2 != null && s.EmerName2.ToLower().Contains(searchLower)) ||
                    (s.EmerName3 != null && s.EmerName3.ToLower().Contains(searchLower))
                );
            }

            // Get total count for pagination
            TotalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
            PageNumber = Math.Max(1, Math.Min(PageNumber, Math.Max(1, TotalPages)));

            // Apply pagination
            ContactRows = await query
                .OrderBy(s => s.Name)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(s => new ContactRow
                {
                    StudentId = s.ID,
                    StudentName = s.Name,
                    EmerName = s.EmerName,
                    EmerRelation = s.EmerRelation,
                    EmerPhone1 = s.EmerPhoneNum1,
                    EmerName2 = s.EmerName2,
                    EmerRelation2 = s.EmerRelation2,
                    EmerPhone2 = s.EmerPhoneNum2,
                    EmerName3 = s.EmerName3,
                    EmerRelation3 = s.EmerRelation3,
                    EmerPhone3 = s.EmerPhoneNum3
                })
                .ToListAsync();

            ViewData["ContactRows"] = ContactRows;
        }
    }
}