using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Manager.Instructors
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2026_Group1_Instructor> Instructors { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }

        // Pagination properties
        public int PageSize { get; set; } = 15;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public async Task OnGetAsync(string? search, string? status)
        {
            SearchTerm = search;
            StatusFilter = status;
            PageNumber = PageNumber > 0 ? PageNumber : 1;

            // Build query with filters
            var query = _context.Spring2026_Group1_Instructor!.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lower = search.ToLower();
                query = query.Where(i =>
                    i.FullName.ToLower().Contains(lower) ||
                    i.Email.ToLower().Contains(lower) ||
                    (i.Speciality != null && i.Speciality.ToLower().Contains(lower)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.Status == status);
            }

            // Get total count for pagination
            TotalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
            PageNumber = Math.Max(1, Math.Min(PageNumber, Math.Max(1, TotalPages)));

            // Apply pagination
            Instructors = await query
                .OrderBy(i => i.FullName)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Get statistics (always from full dataset)
            var all = await _context.Spring2026_Group1_Instructor!.ToListAsync();
            TotalCount = all.Count;
            ActiveCount = all.Count(i => i.Status == "Active");
            InactiveCount = all.Count(i => i.Status != "Active");
        }
    }
}
