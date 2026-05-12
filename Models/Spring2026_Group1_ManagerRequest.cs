using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_ManagerRequest")]
    public class Spring2026_Group1_ManagerRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestID { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Justification { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied

        // Timestamps
        [Column(TypeName = "datetime2")]
        public DateTime RequestDate { get; set; } = DateTime.Now; 

        [Column(TypeName = "datetime2")]
        public DateTime? ReviewDate { get; set; } 

        [StringLength(100)]
        public string? ReviewedBy { get; set; }
    }
}
