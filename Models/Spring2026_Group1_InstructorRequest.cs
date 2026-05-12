using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    [DisplayName("Instructor Request")]
    [Table("Spring2026_Group1_InstructorRequest")]
    public class Spring2026_Group1_InstructorRequest
    {
        [Key]
        public int RequestID { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [StringLength(1000)]
        [Display(Name = "Speciality")]
        public string? Speciality { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Justification")]
        public string Justification { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied

        [Column(TypeName = "datetime2")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2")]
        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Reviewed By")]
        public string? ReviewedBy { get; set; }
    }
}
