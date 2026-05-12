using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_Manager")]
    public class Spring2026_Group1_Manager
    {
        [Key]
        public int ManagerID { get; set; }

        [Required]
        [StringLength(128)]
        public string Email { get; set; }

        public DateTime? HireDate { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? EndDate { get; set; }

        // Navigation properties (optional)
        // public ICollection<Strike> ReviewedStrikes { get; set; }
        // public ICollection<Certificate> IssuedCertificates { get; set; }
    }
}