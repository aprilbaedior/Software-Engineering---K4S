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

namespace CS395SI_Spring2023_Group1.Pages.Admin.EmergencyContacts
{
    [Authorize(Roles = "Admin,SuperAdmin,Manager")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public List<StudentEmergencyContactInfo> StudentContacts { get; set; } = new List<StudentEmergencyContactInfo>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterBy { get; set; } // "All", "HasContacts", "Incomplete"

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalStudents { get; set; }
        public int StudentsWithAllContacts { get; set; }
        public int StudentsWithIncompleteContacts { get; set; }
        public int StudentsWithNoContacts { get; set; }

        // Pagination properties
        public int PageSize { get; set; } = 10; // 10 cards per page
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public async Task OnGetAsync()
        {
            PageNumber = PageNumber > 0 ? PageNumber : 1;

            var profilesQuery = _context.Spring2023_Group1_Profile_Sys.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                profilesQuery = profilesQuery.Where(p =>
                    (p.Name != null && p.Name.Contains(SearchTerm)) ||
                    (p.Email != null && p.Email.Contains(SearchTerm)) ||
                    (p.PhoneNum != null && p.PhoneNum.Contains(SearchTerm))
                );
            }

            var profiles = await profilesQuery
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Build ALL student contact list first (for filtering)
            var allStudentContacts = new List<StudentEmergencyContactInfo>();

            foreach (var profile in profiles)
            {
                var contactInfo = new StudentEmergencyContactInfo
                {
                    StudentName = profile.Name ?? "N/A",
                    StudentEmail = profile.Email ?? "N/A",
                    StudentPhone = profile.PhoneNum ?? "N/A",
                    StudentAddress = profile.Address ?? "N/A",
                    DateOfBirth = profile.DateOfBirth,
                    IsMinor = profile.Under18 == 'Y',
                    GuardianName = profile.PGName,
                    GuardianHomePhone = profile.HomePhone,
                    GuardianMobilePhone = profile.AltPhone,
                    EmergencyContacts = new List<EmergencyContact>()
                };

                // Add emergency contact 1
                if (!string.IsNullOrEmpty(profile.EmerName))
                {
                    contactInfo.EmergencyContacts.Add(new EmergencyContact
                    {
                        Priority = "Primary",
                        Name = profile.EmerName,
                        Relationship = profile.EmerRelation ?? "N/A",
                        PhoneNumber = profile.EmerPhoneNum1 ?? "N/A",
                        Address = profile.EmerAddress ?? "N/A"
                    });
                }

                // Add emergency contact 2
                if (!string.IsNullOrEmpty(profile.EmerName2))
                {
                    contactInfo.EmergencyContacts.Add(new EmergencyContact
                    {
                        Priority = "Secondary",
                        Name = profile.EmerName2,
                        Relationship = profile.EmerRelation2 ?? "N/A",
                        PhoneNumber = profile.EmerPhoneNum2 ?? "N/A",
                        Address = profile.EmerAddress2 ?? "N/A"
                    });
                }

                // Add emergency contact 3
                if (!string.IsNullOrEmpty(profile.EmerName3))
                {
                    contactInfo.EmergencyContacts.Add(new EmergencyContact
                    {
                        Priority = "Tertiary",
                        Name = profile.EmerName3,
                        Relationship = profile.EmerRelation3 ?? "N/A",
                        PhoneNumber = profile.EmerPhoneNum3 ?? "N/A",
                        Address = profile.EmerAddress3 ?? "N/A"
                    });
                }

                // Apply filter
                bool includeStudent = true;
                if (FilterBy == "HasContacts")
                {
                    includeStudent = contactInfo.EmergencyContacts.Count >= 3;
                }
                else if (FilterBy == "Incomplete")
                {
                    includeStudent = contactInfo.EmergencyContacts.Count < 3;
                }

                if (includeStudent)
                {
                    allStudentContacts.Add(contactInfo);
                }
            }

            // Calculate pagination
            TotalRecords = allStudentContacts.Count;
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
            PageNumber = Math.Max(1, Math.Min(PageNumber, Math.Max(1, TotalPages)));

            // Apply pagination - only take 10 cards
            StudentContacts = allStudentContacts
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Calculate statistics from ALL profiles (not filtered)
            TotalStudents = profiles.Count;
            StudentsWithAllContacts = profiles.Count(p => 
                !string.IsNullOrEmpty(p.EmerName) && 
                !string.IsNullOrEmpty(p.EmerName2) && 
                !string.IsNullOrEmpty(p.EmerName3)
            );
            StudentsWithIncompleteContacts = profiles.Count(p => 
                (!string.IsNullOrEmpty(p.EmerName) || 
                 !string.IsNullOrEmpty(p.EmerName2) || 
                 !string.IsNullOrEmpty(p.EmerName3)) &&
                (string.IsNullOrEmpty(p.EmerName) || 
                 string.IsNullOrEmpty(p.EmerName2) || 
                 string.IsNullOrEmpty(p.EmerName3))
            );
            StudentsWithNoContacts = profiles.Count(p => 
                string.IsNullOrEmpty(p.EmerName) && 
                string.IsNullOrEmpty(p.EmerName2) && 
                string.IsNullOrEmpty(p.EmerName3)
            );
        }

        public IActionResult OnPostExport()
        {
            // Existing export logic
            TempData["Info"] = "Export functionality not yet implemented.";
            return RedirectToPage();
        }
    }

    public class StudentEmergencyContactInfo
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public string StudentAddress { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public bool IsMinor { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianHomePhone { get; set; }
        public string? GuardianMobilePhone { get; set; }
        public List<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
    }

    public class EmergencyContact
    {
        public string Priority { get; set; } = string.Empty; // Primary, Secondary, Tertiary
        public string Name { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}