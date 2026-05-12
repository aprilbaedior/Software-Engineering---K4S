using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CS395SI_Spring2023_Group1.Data;

namespace CS395SI_Spring2023_Group1.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public UserRolesModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList Users { get; set; }
        public SelectList Roles { get; set; }

        public List<UserRoleViewModel> UserRolesList { get; set; }
        public string SearchTerm { get; set; }
        public string RoleFilter { get; set; }

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public class InputModel
        {
            [Required]
            public string UserId { get; set; }

            [Required]
            public string RoleName { get; set; }
        }

        public class UserRoleViewModel
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string CurrentRole { get; set; }
        }

        public async Task OnGetAsync(string searchTerm = null, string roleFilter = null, int pageNumber = 1)
        {
            SearchTerm = searchTerm;
            RoleFilter = roleFilter;
            CurrentPage = pageNumber > 0 ? pageNumber : 1;
            PopulateSelectLists();
            await LoadUserRolesAsync(searchTerm, roleFilter, CurrentPage);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                await LoadUserRolesAsync();
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                PopulateSelectLists();
                await LoadUserRolesAsync();
                return Page();
            }

            var isInRole = await _userManager.IsInRoleAsync(user, Input.RoleName);
            if (isInRole)
            {
                ModelState.AddModelError(string.Empty, "User is already in this role");
                PopulateSelectLists();
                await LoadUserRolesAsync();
                return Page();
            }

            var result = await _userManager.AddToRoleAsync(user, Input.RoleName);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role '{Input.RoleName}' assigned to {user.Email} successfully.";
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            PopulateSelectLists();
            await LoadUserRolesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(string userId, string roleName, string searchTerm, string roleFilter, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                TempData["ErrorMessage"] = "User ID and Role are required.";
                return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
            }

            // Get current roles and remove them
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Add the new role
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role for {user.Email} updated to {roleName}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error updating role: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(string userId, string searchTerm, string roleFilter, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Set a default password
            string newPassword = "Password123!";

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Password for {user.Email} has been reset to: {newPassword}";
            }
            else
            {
                TempData["ErrorMessage"] = "Error resetting password: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId, string searchTerm, string roleFilter, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.Email} has been deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToPage(new { searchTerm, roleFilter, pageNumber });
        }

        private void PopulateSelectLists()
        {
            // Create lists to avoid multiple database calls
            var usersList = _userManager.Users.OrderBy(u => u.Email).ToList();
            var rolesList = _roleManager.Roles.OrderBy(r => r.Name).ToList();

            Users = new SelectList(usersList, "Id", "Email");
            Roles = new SelectList(rolesList, "Name", "Name");
        }

        private async Task LoadUserRolesAsync(string searchTerm = null, string roleFilter = null, int pageNumber = 1)
        {
            var allUserRoles = new List<UserRoleViewModel>();

            // Get all users first
            var allUsers = await _userManager.Users.ToListAsync();

            // Get all profile data in a single query to improve performance
            var userEmails = allUsers.Select(u => u.Email).ToList();
            var profiles = await _context.Spring2023_Group1_Profile_Sys
                .Where(p => userEmails.Contains(p.Email))
                .ToListAsync();

            var profileDict = profiles.ToDictionary(p => p.Email, p => p.Name);

            // Process each user
            foreach (var user in allUsers)
            {
                // Get roles for this user
                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.Any() ? string.Join(", ", roles) : "No Role";

                // Apply role filter if provided
                if (!string.IsNullOrEmpty(roleFilter) && !roles.Contains(roleFilter))
                {
                    continue; // Skip this user if they don't have the filtered role
                }

                // Try to get user's name from profiles
                string userName = user.Email; // Default to email
                if (profileDict.TryGetValue(user.Email, out string name) && !string.IsNullOrEmpty(name))
                {
                    userName = name;
                }

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    bool nameMatch = userName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                    bool emailMatch = user.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);

                    if (!nameMatch && !emailMatch)
                    {
                        continue; // Skip this user if they don't match the search term
                    }
                }

                allUserRoles.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = userName,
                    CurrentRole = currentRole
                });
            }

            // Sort the list by name
            allUserRoles = allUserRoles.OrderBy(u => u.Name).ToList();

            // Calculate pagination
            TotalRecords = allUserRoles.Count;
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);

            // Ensure current page is within valid range
            CurrentPage = Math.Max(1, Math.Min(pageNumber, Math.Max(1, TotalPages)));

            // Apply pagination
            UserRolesList = allUserRoles
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}