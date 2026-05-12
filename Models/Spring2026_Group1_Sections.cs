using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_K4S.Model
{
    [Table("Spring2026_Group1_Sections")]
    public class Spring2026_Group1_Sections
    {
        [Column(TypeName = "char(16)")]
        [StringLength(16)]
        public string? ServiceID { get; set; }

        [StringLength(64)]
        public string? ServiceName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [StringLength(100)]
        public string? WeekDay { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? StartTime { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? EndTime { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SectionID { get; set; }

        public int Capacity { get; set; }

        // Foreign key to Spring2026_Group1_Instructor.InstructorID - NOW NULLABLE
        [Column("instructorID")]
        public int? InstructorID { get; set; }

        [ForeignKey("InstructorID")]
        public Spring2026_Group1_Instructor? Instructor { get; set; }
    }
}
