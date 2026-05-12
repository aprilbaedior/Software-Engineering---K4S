using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_Students")]
    public class Spring2026_Group1_Students
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        [StringLength(128)]
        public string Email { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? EndDate { get; set; }

        // Navigation properties (optional)
        // public ICollection<Strike> Strikes { get; set; }
        // public ICollection<Certificate> Certificates { get; set; }
    }
}

