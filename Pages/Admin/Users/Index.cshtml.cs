using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS395SI_Spring2023_Group1.Data;

namespace CS395SI_Spring2023_Group1.Pages.Admin.Users
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserWithRoles> Users { get; set; } = new List<UserWithRoles>();
        
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public List<string> AvailableRoles { get; set; } = new List<string>();

        // Pagination properties
        public int PageSize { get; set; } = 15;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public async Task OnGetAsync()
        {
            PageNumber = PageNumber > 0 ? PageNumber : 1;

            // Get all roles for filter dropdown
            AvailableRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

            // Get all users - apply search filter first to reduce dataset
            IQueryable<IdentityUser> userQuery = _userManager.Users;

            // Apply search filter at database level
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                userQuery = userQuery.Where(u =>
                    (u.Email != null && u.Email.Contains(SearchTerm)) ||
                    (u.UserName != null && u.UserName.Contains(SearchTerm))
                );
            }

            var filteredUsers = await userQuery.OrderBy(u => u.Email).ToListAsync();

            // Build user list with roles
            var allUserRoles = new List<UserWithRoles>();

            foreach (var user in filteredUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Apply role filter
                if (!string.IsNullOrWhiteSpace(RoleFilter) && RoleFilter != "All")
                {
                    if (!roles.Contains(RoleFilter))
                        continue;
                }

                allUserRoles.Add(new UserWithRoles
                {
                    Id = user.Id,
                    Email = user.Email ?? "N/A",
                    UserName = user.UserName ?? "N/A",
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnd = user.LockoutEnd,
                    IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow,
                    Roles = roles.ToList()
                });
            }

            // Calculate pagination
            TotalRecords = allUserRoles.Count;
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
            PageNumber = Math.Max(1, Math.Min(PageNumber, Math.Max(1, TotalPages)));

            // Apply pagination
            Users = allUserRoles
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public async Task<IActionResult> OnPostToggleLockoutAsync(string userId, int pageNumber = 1, string? searchTerm = null, string? roleFilter = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToPage(new { PageNumber = pageNumber, SearchTerm = searchTerm, RoleFilter = roleFilter });
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                // Unlock user
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"User {user.Email} has been unlocked.";
            }
            else
            {
                // Lock user for 100 years
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                TempData["Success"] = $"User {user.Email} has been locked out.";
            }

            return RedirectToPage(new { PageNumber = pageNumber, SearchTerm = searchTerm, RoleFilter = roleFilter });
        }
    }

    public class UserWithRoles
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool IsLockedOut { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}