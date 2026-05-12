using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_Certificate")]
    public class Spring2026_Group1_Certificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CertificateID { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Student Email")]
        public string StudentEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Service ID")]
        public string ServiceID { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Service Name")]
        public string ServiceName { get; set; } = string.Empty;

        [Display(Name = "Section ID")]
        public int SectionID { get; set; }

        [Display(Name = "Issued Date")]
        public DateTime IssuedDate { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [StringLength(128)]
        [Display(Name = "Student ID")]
        public string? StudentID { get; set; }

        [StringLength(128)]
        [Display(Name = "Manager ID")]
        public string? ManagerID { get; set; }

        [StringLength(50)]
        [Display(Name = "Certificate Status")]
        public string? CertificateStatus { get; set; }
    }
}
