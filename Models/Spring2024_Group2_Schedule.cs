using System.ComponentModel.DataAnnotations;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2024_Group2_Schedule
    {
        [Key]
        public int ScheduleID { get; set; }
        public string? StudentEmail { get; set; }
        public string? ServiceID { get; set; }
        public string? ServiceName { get; set; }
        public int SectionID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? WeekDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Status { get; set; }
    }
}
