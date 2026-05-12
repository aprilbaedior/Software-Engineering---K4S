using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2023_Group1_Schedules
    {
        public string Email { get; set; }
        public string? ServiceID { get; set; }
        public int? SectionID { get; set; }
        public string? ScheduleStatus { get; set; }
        [Key]
        public int ScheduleID { get; set; }
    }
}
