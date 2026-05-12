using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_Instructor")]
    public class Spring2026_Group1_Instructor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstructorID { get; set; }

        [Required]
        [StringLength(128)]
        public string Email { get; set; }

        // added full name field 
        [Required]
        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public DateTime? HireDate { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(1000)]
        public string? Speciality { get; set; }

        // Navigation properties (optional)
        // public ICollection<Strike> Strikes { get; set; }

        // Manager approval fields
        [Required]
        [Display(Name = "Approved Date")]
        public DateTime ApprovedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(128)]
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
    }
}
