using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    [DisplayName("Attendance Record")]
    public class Spring2025_Group3_Attendance
    {
        [Key]
        [StringLength(16)]
        public int AttendanceID { get; set; } 

        [Required]
        [EmailAddress]
        [StringLength(128)]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(16)]
        public string ServiceID { get; set; }

        [Required]
        public int SectionID { get; set; }

        [Required]
        public int ScheduleID { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CurrentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        [Display(Name = "Attendance Status")]
        public string AttendanceStatus { get; set; }
    }
}
