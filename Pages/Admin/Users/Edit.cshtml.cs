using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Admin.Users
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CS395SI_Spring2023_Group1Context _context;

        public EditModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            CS395SI_Spring2023_Group1Context context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public UserEditModel Input { get; set; } = new UserEditModel();

        [BindProperty]
        public ProfileEditModel? ProfileInput { get; set; }

        [BindProperty]
        public EmployeeEditModel? EmployeeInput { get; set; }

        public List<RoleCheckbox> AllRoles { get; set; } = new List<RoleCheckbox>();

        public string ProfileType { get; set; } = "None"; // "Student", "Employee", or "None"

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input.Id = user.Id;
            Input.Email = user.Email ?? string.Empty;
            Input.UserName = user.UserName ?? string.Empty;
            Input.EmailConfirmed = user.EmailConfirmed;
            Input.PhoneNumber = user.PhoneNumber;

            // Load user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            foreach (var role in allRoles)
            {
                AllRoles.Add(new RoleCheckbox
                {
                    RoleName = role.Name!,
                    IsSelected = userRoles.Contains(role.Name!)
                });
            }

            // Determine profile type based on roles
            bool isStaffRole = userRoles.Any(r => r == "Instructor" || r == "Manager" || r == "Admin" || r == "SuperAdmin");
            bool isStudentRole = userRoles.Contains("Student");

            // Priority: If user has staff roles, load employee application
            // If only student role, load student profile
            if (isStaffRole)
            {
                await LoadEmployeeApplicationAsync(user.Email);
            }
            else if (isStudentRole)
            {
                await LoadStudentProfileAsync(user.Email);
            }
            else
            {
                // No role assigned yet, check what data exists
                var hasEmployeeApp = await _context.Spring2026_Group1_EmployeeApplication
                    .AnyAsync(e => e.Email == user.Email);
                var hasStudentProfile = await _context.Spring2023_Group1_Profile_Sys
                    .AnyAsync(p => p.Email == user.Email);

                if (hasEmployeeApp)
                {
                    await LoadEmployeeApplicationAsync(user.Email);
                }
                else if (hasStudentProfile)
                {
                    await LoadStudentProfileAsync(user.Email);
                }
                else
                {
                    ProfileType = "None";
                }
            }

            return Page();
        }

        private async Task LoadStudentProfileAsync(string? email)
        {
            var profile = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(p => p.Email == email);

            if (profile != null)
            {
                ProfileType = "Student";
                ProfileInput = new ProfileEditModel
                {
                    Name = profile.Name,
                    PhoneNum = profile.PhoneNum,
                    Address = profile.Address,
                    DateOfBirth = profile.DateOfBirth,
                    Sex = profile.Sex,
                    Race = profile.Race,
                    Under18 = profile.Under18,
                    PGName = profile.PGName,
                    HomePhone = profile.HomePhone,
                    AltPhone = profile.AltPhone,
                    SchoolName = profile.SchoolName,
                    Grade = profile.Grade,
                    BusinessName = profile.BusinessName,
                    EmerName = profile.EmerName,
                    EmerRelation = profile.EmerRelation,
                    EmerPhoneNum1 = profile.EmerPhoneNum1,
                    EmerAddress = profile.EmerAddress,
                    EmerName2 = profile.EmerName2,
                    EmerRelation2 = profile.EmerRelation2,
                    EmerPhoneNum2 = profile.EmerPhoneNum2,
                    EmerAddress2 = profile.EmerAddress2,
                    EmerName3 = profile.EmerName3,
                    EmerRelation3 = profile.EmerRelation3,
                    EmerPhoneNum3 = profile.EmerPhoneNum3,
                    EmerAddress3 = profile.EmerAddress3,
                    ApplicationStatus = profile.ApplicationStatus
                };
            }
        }

        private async Task LoadEmployeeApplicationAsync(string? email)
        {
            var application = await _context.Spring2026_Group1_EmployeeApplication
                .FirstOrDefaultAsync(e => e.Email == email);

            if (application != null)
            {
                ProfileType = "Employee";
                EmployeeInput = new EmployeeEditModel
                {
                    FullName = application.FullName,
                    PhoneNumber = application.PhoneNumber,
                    Address = application.Address,
                    DateOfBirth = application.DateOfBirth,
                    DesiredPosition = application.DesiredPosition,
                    Qualifications = application.Qualifications,
                    Motivation = application.Motivation,
                    Specialties = application.Specialties,
                    ApplicationStatus = application.ApplicationStatus,
                    ApplicationDate = application.ApplicationDate,
                    ReviewNotes = application.ReviewNotes
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var allRoles = await _roleManager.Roles.ToListAsync();
                foreach (var role in allRoles)
                {
                    AllRoles.Add(new RoleCheckbox { RoleName = role.Name!, IsSelected = false });
                }
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Update phone number
            if (user.PhoneNumber != Input.PhoneNumber)
            {
                user.PhoneNumber = Input.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            // Update email confirmation
            if (Input.EmailConfirmed != user.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (Input.EmailConfirmed)
                {
                    await _userManager.ConfirmEmailAsync(user, token);
                }
            }

            // Update roles
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = Input.SelectedRoles ?? new List<string>();

            var rolesToRemove = userRoles.Except(selectedRoles).ToList();
            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            var rolesToAdd = selectedRoles.Except(userRoles).ToList();
            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            // Update profile data based on type
            if (ProfileInput != null && !string.IsNullOrEmpty(Input.ProfileType))
            {
                if (Input.ProfileType == "Student")
                {
                    // Convert checkbox value: 'Y' if checked, 'N' if unchecked
                    if (ProfileInput.Under18 == 'Y')
                    {
                        // Already 'Y', keep it
                    }
                    else if (ProfileInput.Under18 == null || ProfileInput.Under18 == '\0')
                    {
                        // Checkbox was unchecked (null or empty char)
                        ProfileInput.Under18 = 'N';
                    }
                    
                    await UpdateStudentProfileAsync(user.Email, ProfileInput);
                }
            }

            if (EmployeeInput != null && !string.IsNullOrEmpty(Input.ProfileType))
            {
                if (Input.ProfileType == "Employee")
                {
                    await UpdateEmployeeApplicationAsync(user.Email, EmployeeInput);
                }
            }

            TempData["Success"] = $"User {user.Email} has been updated successfully.";
            return RedirectToPage("/Admin/Users/Index");
        }

        private async Task UpdateStudentProfileAsync(string? email, ProfileEditModel input)
        {
            var profile = await _context.Spring2023_Group1_Profile_Sys
                .FirstOrDefaultAsync(p => p.Email == email);

            if (profile != null)
            {
                profile.Name = input.Name ?? profile.Name;
                profile.PhoneNum = input.PhoneNum;
                profile.Address = input.Address ?? profile.Address;
                profile.DateOfBirth = input.DateOfBirth;
                profile.Sex = input.Sex;
                profile.Race = input.Race;
                profile.Under18 = input.Under18;
                profile.PGName = input.PGName;
                profile.HomePhone = input.HomePhone;
                profile.AltPhone = input.AltPhone;
                profile.SchoolName = input.SchoolName;
                profile.Grade = input.Grade;
                profile.BusinessName = input.BusinessName;
                profile.EmerName = input.EmerName;
                profile.EmerRelation = input.EmerRelation;
                profile.EmerPhoneNum1 = input.EmerPhoneNum1;
                profile.EmerAddress = input.EmerAddress;
                profile.EmerName2 = input.EmerName2;
                profile.EmerRelation2 = input.EmerRelation2;
                profile.EmerPhoneNum2 = input.EmerPhoneNum2;
                profile.EmerAddress2 = input.EmerAddress2;
                profile.EmerName3 = input.EmerName3;
                profile.EmerRelation3 = input.EmerRelation3;
                profile.EmerPhoneNum3 = input.EmerPhoneNum3;
                profile.EmerAddress3 = input.EmerAddress3;
                profile.ApplicationStatus = input.ApplicationStatus ?? profile.ApplicationStatus;

                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateEmployeeApplicationAsync(string? email, EmployeeEditModel input)
        {
            var application = await _context.Spring2026_Group1_EmployeeApplication
                .FirstOrDefaultAsync(e => e.Email == email);

            if (application != null)
            {
                application.FullName = input.FullName ?? application.FullName;
                application.PhoneNumber = input.PhoneNumber ?? application.PhoneNumber;
                application.Address = input.Address ?? application.Address;
                application.DateOfBirth = input.DateOfBirth;
                application.DesiredPosition = input.DesiredPosition ?? application.DesiredPosition;
                application.Qualifications = input.Qualifications;
                application.Motivation = input.Motivation;
                application.Specialties = input.Specialties;
                application.ApplicationStatus = input.ApplicationStatus ?? application.ApplicationStatus;
                application.ReviewNotes = input.ReviewNotes;
                application.ReviewedBy = User.Identity?.Name;
                application.ReviewDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(string userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["Error"] = "Password must be at least 6 characters long.";
                return RedirectToPage(new { id = userId });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToPage("/Admin/Users/Index");
            }

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, newPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Password for {user.Email} has been reset successfully.";
            }
            else
            {
                TempData["Error"] = $"Failed to reset password: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToPage(new { id = userId });
        }
    }

    public class UserEditModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string>? SelectedRoles { get; set; }
        public string? ProfileType { get; set; } // To track which profile is being edited
    }

    public class ProfileEditModel
    {
        public string? Name { get; set; }
        public string? PhoneNum { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public char? Sex { get; set; }
        public string? Race { get; set; }
        public char? Under18 { get; set; }
        public string? PGName { get; set; }
        public string? HomePhone { get; set; }
        public string? AltPhone { get; set; }
        public string? SchoolName { get; set; }
        public int? Grade { get; set; }
        public string? BusinessName { get; set; }
        public string? EmerName { get; set; }
        public string? EmerRelation { get; set; }
        public string? EmerPhoneNum1 { get; set; }
        public string? EmerAddress { get; set; }
        public string? EmerName2 { get; set; }
        public string? EmerRelation2 { get; set; }
        public string? EmerPhoneNum2 { get; set; }
        public string? EmerAddress2 { get; set; }
        public string? EmerName3 { get; set; }
        public string? EmerRelation3 { get; set; }
        public string? EmerPhoneNum3 { get; set; }
        public string? EmerAddress3 { get; set; }
        public string? ApplicationStatus { get; set; }
    }

    public class EmployeeEditModel
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? DesiredPosition { get; set; }
        public string? Qualifications { get; set; }
        public string? Motivation { get; set; }
        public string? Specialties { get; set; }
        public string? ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string? ReviewNotes { get; set; }
    }

    public class RoleCheckbox
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}