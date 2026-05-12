using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2026_Group1_Strike
    {
        [Key]
        public int StrikeID { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128)]
        [Display(Name = "Filed By (Instructor Email)")]
        public string FiledBy { get; set; }

        // Optional fields for context
        [StringLength(50)]
        [Display(Name = "Service ID")]
        public string? ServiceID { get; set; }

        [Display(Name = "Section ID")]
        public int? SectionID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128)]
        [Display(Name = "Student Email")]
        public string StudentEmail { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Reason")]
        public string Reason { get; set; }

        [Required]
        [Display(Name = "Filed Date")]
        public DateTime FiledDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string ReviewStatus { get; set; } = "Pending"; // Pending / Approved / Denied

        [StringLength(128)]
        [Display(Name = "Reviewed By")]
        public string? ReviewedBy { get; set; }

        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Manager Notes")]
        public string? ManagerNotes { get; set; }


        // Navigation properties (optional)
        // public Student Student { get; set; }
        // public Instructor Instructor { get; set; }
        // public Manager Manager { get; set; }

        // Added system fields used by Create.cshtml.cs
        public int? InstructorID { get; set; }    // <- add this
        public int? ManagerID { get; set; }
    }
}
