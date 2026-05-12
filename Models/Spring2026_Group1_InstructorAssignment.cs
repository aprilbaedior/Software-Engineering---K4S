using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CS395SI_Spring2023_K4S.Model
{
    [DisplayName("Instructor Assignment")]
    public class Spring2026_Group1_InstructorAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128)]
        [Display(Name = "Instructor Email")]
        public string InstructorEmail { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        [Required]
        [Display(Name = "Section ID")]
        public int SectionID { get; set; }

        [StringLength(200)]
        [Display(Name = "Service Name")]
        public string? ServiceName { get; set; }

        [Required]
        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(128)]
        [Display(Name = "Assigned By")]
        public string AssignedBy { get; set; }
    }
}