using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CS395SI_Spring2023_K4S.Model
{
    [DisplayName("Registration Form")]
    public class Spring2023_Group1_Profile_Sys
    {
        // All of the nullable types (e.g. string?, DateTime?) are variables that don't need to be filled out.
        public int ID { get; set; } 
        [Display(Name = "Name")]
        public string Name              { get; set; }
        [Key]
        [Display(Name = "E-Mail")]
        public string Email             { get; set; }

        public string? Password          { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNum          { get; set; }
        [Display(Name = "Home Address")]
        public string Address           { get; set; }
        [Display(Name = "License Number")]
        public string? LicenseNum        { get; set; }
        [Display(Name = "License State")]
        public string? LicenseState      { get; set; }
        [Display(Name = "License Issue Date")]
        public DateTime? LicenseIssue    { get; set; }
        [Display(Name = "License Expiration Date")]
        public DateTime? LicenseExp      { get; set; }
        [Display(Name = "License Class")]
        public string? LicenseClass      { get; set; }
        [Display(Name = "Under 18? (Y/N)")]
        public char? Under18             { get; set; }
        [Display(Name = "Parent/Guardian Name")]
        public string? PGName            { get; set; }
        [Display(Name = "Parent/Guardian Home Phone")]
        public string? HomePhone         { get; set; }
        [Display(Name = "Parent/Guardian Mobile Phone")]
        public string? AltPhone          { get; set; }
        [Display(Name = "US Citizen? (Y/N)")]
        public char? USCitizen           { get; set; }
        [Display(Name = "Race")]
        public string? Race              { get; set; }
        [Display(Name = "Sex (M/F)")]
        public char? Sex                 { get; set; }
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth     { get; set; }
        [Display(Name = "High School Graduate? (Y/N)")]
        public char? HSGrad              { get; set; }
        [Display(Name = "High School Name")]
        public string? SchoolName        { get; set; }
        [Display(Name = "High School Address")]
        public string? SchoolAddress     { get; set; }
        [Display(Name = "Grade")]
        public int? Grade { get; set; }
        [Display(Name = "Employed? (Y/N)")]
        public char? Employed { get; set; }
        [Display(Name = "Business Name")]
        public string? BusinessName { get; set; }
        [Display(Name = "Business Address")]
        public string? BusinessAddress { get; set; }
        [Display(Name = "Business Phone")]
        public string? BusinessPhone { get; set; }
        [Display(Name = "Talents")]
        public string? Talents { get; set; }
        [Display(Name = "Emergency Name")]
        public string? EmerName { get; set; }
        [Display(Name = "Emergency Name")]
        public string? EmerName2 { get; set; }
        [Display(Name = "Emergency Name")]
        public string? EmerName3 { get; set; }
        [Display(Name = "Emergency Relation")]
        public string? EmerRelation { get; set; }
        [Display(Name = "Emergency Relation")]
        public string? EmerRelation2 { get; set; }
        [Display(Name = "Emergency Relation")]
        public string? EmerRelation3 { get; set; }
        [Display(Name = "Emergency Phone Number")]
        public string? EmerPhoneNum1 { get; set; }
        [Display(Name = "Emergency Phone Number")]
        public string? EmerPhoneNum2 { get; set; }
        [Display(Name = "Emergency Phone Number")]
        public string? EmerPhoneNum3 { get; set; }
        [Display(Name = "Emergency Address")]
        public string? EmerAddress { get; set; }
        [Display(Name = "Emergency Address")]
        public string? EmerAddress2 { get; set; }
        [Display(Name = "Emergency Address")]
        public string? EmerAddress3 { get; set; }
        [Display(Name = "Status")]
        public string ApplicationStatus { get; set; } = "Pending"; // Pending, Approved or Denied
    }

}
