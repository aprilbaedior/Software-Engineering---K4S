using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace CS395SI_Spring2023_Group1.Pages.Manager.Students
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<Spring2023_Group1_Profile_Sys> Students { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        public int TotalStudents { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int DeniedCount { get; set; }

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
            var query = _context.Spring2023_Group1_Profile_Sys.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lower = search.ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(lower) ||
                    s.Email.ToLower().Contains(lower));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.ApplicationStatus == status);
            }

            // Get total count for pagination
            TotalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
            PageNumber = Math.Max(1, Math.Min(PageNumber, Math.Max(1, TotalPages)));

            // Apply pagination
            Students = await query
                .OrderBy(s => s.Name)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Get statistics (always from full dataset)
            var all = await _context.Spring2023_Group1_Profile_Sys.ToListAsync();
            TotalStudents = all.Count;
            ApprovedCount = all.Count(s => s.ApplicationStatus == "Approved");
            PendingCount = all.Count(s => s.ApplicationStatus == "Pending");
            DeniedCount = all.Count(s => s.ApplicationStatus == "Denied");
        }
    }
}