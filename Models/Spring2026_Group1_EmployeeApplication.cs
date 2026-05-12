using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_EmployeeApplication")]
    [DisplayName("Faculty Application")]
    public class Spring2026_Group1_EmployeeApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Home Address")]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Desired Position")]
        public string DesiredPosition { get; set; } // "Instructor", "Manager", or "Administrator"

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(1000)]
        [Display(Name = "Relevant Experience & Qualifications")]
        [DataType(DataType.MultilineText)]
        public string? Qualifications { get; set; }

        [StringLength(1000)]
        [Display(Name = "Why do you want to work with Krumpin 4 Success?")]
        [DataType(DataType.MultilineText)]
        public string? Motivation { get; set; }

        [StringLength(500)]
        [Display(Name = "Areas of Expertise")]
        public string? Specialties { get; set; }

        // Application tracking
        [Required]
        [StringLength(50)]
        [Display(Name = "Application Status")]
        public string ApplicationStatus { get; set; } = "Pending"; // Pending, Approved, Denied

        [Display(Name = "Application Date")]
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        [StringLength(128)]
        [Display(Name = "Reviewed By")]
        public string? ReviewedBy { get; set; }

        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }

        [StringLength(1000)]
        [Display(Name = "Review Notes")]
        [DataType(DataType.MultilineText)]
        public string? ReviewNotes { get; set; }
    }
}