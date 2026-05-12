using System.ComponentModel.DataAnnotations;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2024_Group2_Session
    {
        [Key]
        public int SessionID { get; set; }
        public int? ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int? SectionID { get; set; }
        public DateTime? MeetingDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Status { get; set; }
    }
}
