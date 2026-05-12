using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_Schedule")]
    public class Spring2026_Group1_Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScheduleID { get; set; }

        [Required]
        [StringLength(16)]
        public string ServiceID { get; set; } = null!;

        [StringLength(64)]
        public string? ServiceName { get; set; }

        [Required]
        public int SectionID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(10)]
        public string? WeekDay { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128)]
        public string StudentEmail { get; set; } = null!;

        public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;

        // Foreign key to Spring2026_Group1_Sections
        [ForeignKey("SectionID")]
        public Spring2026_Group1_Sections? Section { get; set; }
    }
}