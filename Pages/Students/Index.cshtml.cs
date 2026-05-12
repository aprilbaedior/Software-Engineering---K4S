using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages_Students
{
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public IList<StudentViewModel> Spring2026_Group1_Students { get; set; } = default!;

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        // Search properties
        public string SearchTerm { get; set; }
        public string StatusFilter { get; set; }

        // ViewModel to combine student and profile data
        public class StudentViewModel
        {
            public int StudentID { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public DateTime? EnrollmentDate { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public async Task OnGetAsync(string searchTerm = null, string statusFilter = null, int pageNumber = 1)
        {
            SearchTerm = searchTerm;
            StatusFilter = statusFilter;
            CurrentPage = pageNumber > 0 ? pageNumber : 1;

            // Start with base query - join students with profiles to get names
            var query = from student in _context.Spring2026_Group1_Students
                        join profile in _context.Spring2023_Group1_Profile_Sys
                        on student.Email equals profile.Email into profileGroup
                        from profile in profileGroup.DefaultIfEmpty()
                        select new StudentViewModel
                        {
                            StudentID = student.StudentID,
                            Email = student.Email,
                            Name = profile != null ? profile.Name : student.Email,
                            EnrollmentDate = student.EnrollmentDate,
                            IsActive = student.IsActive,
                            EndDate = student.EndDate
                        };

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s =>
                    s.Email.Contains(searchTerm) ||
                    s.Name.Contains(searchTerm));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.IsActive == true);
                }
                else if (statusFilter.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.IsActive == false || s.IsActive == null);
                }
            }

            // Get total count for pagination
            TotalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);

            // Ensure current page is within valid range
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, Math.Max(1, TotalPages)));

            // Apply pagination and get results
            Spring2026_Group1_Students = await query
                .OrderBy(s => s.Name)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
