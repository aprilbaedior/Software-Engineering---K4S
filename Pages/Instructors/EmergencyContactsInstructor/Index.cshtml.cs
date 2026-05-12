using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;


namespace CS395SI_Spring2023_Group1.Pages.Instructors.EmergencyContacts
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;
        public IndexModel(CS395SI_Spring2023_Group1Context context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IList<Spring2026_Group1_Sections> AssignedSections { get; set; } = new List<Spring2026_Group1_Sections>();

        public List<StudentEmergencyContactInfo> StudentContacts { get; set; } = new List<StudentEmergencyContactInfo>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterBy { get; set; } // "All", "HasContacts", "Incomplete"

        [BindProperty(SupportsGet = true)]
        public int? SectionId { get; set; } // optional: restrict to one assigned section

        public async Task OnGetAsync()
        {
            // Resolve current instructor
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                StudentContacts = new List<StudentEmergencyContactInfo>();
                return;
            }

            var instructor = await _context.Spring2026_Group1_Instructor
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            if (instructor == null)
            {
                StudentContacts = new List<StudentEmergencyContactInfo>();
                return;
            }

            // Load assigned sections for UI/filtering
            if (_context.Spring2026_Group1_Sections != null)
            {
                AssignedSections = await _context.Spring2026_Group1_Sections
                    .Where(s => s.InstructorID == instructor.InstructorID)
                    .OrderBy(s => s.ServiceName)
                    .ThenBy(s => s.StartTime)
                    .ToListAsync();
            }

            var assignedSectionIds = AssignedSections
                .Select(s => s.SectionID)
                .ToList();

            if (SectionId.HasValue)
            {
                // Ensure the requested section belongs to this instructor
                if (!assignedSectionIds.Contains(SectionId.Value))
                {
                    StudentContacts = new List<StudentEmergencyContactInfo>();
                    return;
                }

                assignedSectionIds = new List<int> { SectionId.Value };
            }

            if (assignedSectionIds.Count == 0)
            {
                StudentContacts = new List<StudentEmergencyContactInfo>();
                return;
            }

            // Get distinct student emails enrolled in those sections
            var studentEmails = await _context.Spring2026_Group1_Schedule
                .Where(s => assignedSectionIds.Contains(s.SectionID))
                .Select(s => s.StudentEmail)
                .Distinct()
                .ToListAsync();

            if (studentEmails.Count == 0)
            {
                StudentContacts = new List<StudentEmergencyContactInfo>();
                return;
            }

            // Query profiles for those students
            var profilesQuery = _context.Spring2023_Group1_Profile_Sys
                .AsQueryable()
                .Where(p => p.Email != null && studentEmails.Contains(p.Email));

            // Apply search filter if provided
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
                    StudentContacts.Add(contactInfo);
                }
            }
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
        public string Priority { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    }

