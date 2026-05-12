// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            
            [Display(Name = "Full Name")]
            public string FullName { get; set; }
            
            [Display(Name = "Address")]
            public string Address { get; set; }
            
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime? DateOfBirth { get; set; }
            
            [Display(Name = "High School Graduate")]
            public char? HSGrad { get; set; }
            
            [Display(Name = "School Name")]
            public string SchoolName { get; set; }
            
            [Display(Name = "School Address")]
            public string SchoolAddress { get; set; }
            
            [Display(Name = "Grade/Year")]
            public int? Grade { get; set; }
            
            [Display(Name = "Currently Employed")]
            public char? Employed { get; set; }
            
            [Display(Name = "Employer/Business Name")]
            public string BusinessName { get; set; }
            
            [Display(Name = "Business Address")]
            public string BusinessAddress { get; set; }
            
            [Display(Name = "Skills & Talents")]
            public string Talents { get; set; }
            
            [Display(Name = "Emergency Contact Name")]
            public string EmerName { get; set; }
            
            [Display(Name = "Emergency Contact Relationship")]
            public string EmerRelation { get; set; }
            
            [Phone]
            [Display(Name = "Emergency Contact Phone")]
            public string EmerPhoneNum1 { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            // Get additional profile information from your database
            var profile = await _context.Spring2023_Group1_Profile_Sys.FirstOrDefaultAsync(p => p.Email == userName);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                // Populate additional fields if profile exists
                FullName = profile?.Name,
                Address = profile?.Address,
                DateOfBirth = profile?.DateOfBirth,
                HSGrad = profile?.HSGrad,
                SchoolName = profile?.SchoolName,
                SchoolAddress = profile?.SchoolAddress,
                Grade = profile?.Grade,
                Employed = profile?.Employed,
                BusinessName = profile?.BusinessName,
                BusinessAddress = profile?.BusinessAddress,
                Talents = profile?.Talents,
                EmerName = profile?.EmerName,
                EmerRelation = profile?.EmerRelation,
                EmerPhoneNum1 = profile?.EmerPhoneNum1
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            
            // Update additional profile information in your database
            var profile = await _context.Spring2023_Group1_Profile_Sys.FirstOrDefaultAsync(p => p.Email == user.Email);
            
            // Check if profile exists
            if (profile == null)
            {
                // Create a new profile - define the profile class with its properties
                var newProfile = new 
                {
                    Email = user.Email,
                    Name = Input.FullName,
                    Address = Input.Address,
                    DateOfBirth = Input.DateOfBirth,
                    HSGrad = Input.HSGrad,
                    SchoolName = Input.SchoolName,
                    SchoolAddress = Input.SchoolAddress,
                    Grade = Input.Grade ?? 0,
                    Employed = Input.Employed,
                    BusinessName = Input.BusinessName,
                    BusinessAddress = Input.BusinessAddress,
                    Talents = Input.Talents,
                    EmerName = Input.EmerName,
                    EmerRelation = Input.EmerRelation,
                    EmerPhoneNum1 = Input.EmerPhoneNum1
                };
                
                _context.Spring2023_Group1_Profile_Sys.Add(
                    (dynamic)newProfile
                );
            }
            else
            {
                // Update existing profile
                profile.Name = Input.FullName;
                profile.Address = Input.Address;
                profile.DateOfBirth = Input.DateOfBirth;
                profile.HSGrad = Input.HSGrad;
                profile.SchoolName = Input.SchoolName;
                profile.SchoolAddress = Input.SchoolAddress;
                profile.Grade = Input.Grade ?? 0;
                profile.Employed = Input.Employed;
                profile.BusinessName = Input.BusinessName;
                profile.BusinessAddress = Input.BusinessAddress;
                profile.Talents = Input.Talents;
                profile.EmerName = Input.EmerName;
                profile.EmerRelation = Input.EmerRelation;
                profile.EmerPhoneNum1 = Input.EmerPhoneNum1;
            }
            
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}