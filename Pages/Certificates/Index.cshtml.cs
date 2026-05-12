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
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public IList<Spring2026_Group1_Certificate> Spring2026_Group1_Certificate { get; set; } = default!;

        // Pagination and filtering
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ServiceFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 15;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public List<string> AvailableServices { get; set; } = new();

        public async Task OnGetAsync()
        {
            PageNumber = PageNumber > 0 ? PageNumber : 1;

            // Get available services for filter
            AvailableServices = await _context.Spring2026_Group1_Certificate
                .Select(c => c.ServiceName)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            // Build query
            var query = _context.Spring2026_Group1_Certificate.AsQueryable();

            // Filter by role
            if (User.IsInRole("Student"))
            {
                var email = User.Identity?.Name ?? "";
                query = query.Where(c => c.StudentEmail == email);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(c =>
                    c.StudentName.Contains(SearchTerm) ||
                    c.StudentEmail.Contains(SearchTerm) ||
                    c.ServiceName.Contains(SearchTerm));
            }

            // Apply service filter
            if (!string.IsNullOrWhiteSpace(ServiceFilter) && ServiceFilter != "All")
            {
                query = query.Where(c => c.ServiceName == ServiceFilter);
            }

            // Calculate pagination
            TotalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
            PageNumber = Math.Max(1, Math.Min(PageNumber, Math.Max(1, TotalPages)));

            // Apply pagination and get results
            Spring2026_Group1_Certificate = await query
                .OrderByDescending(c => c.IssuedDate)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
