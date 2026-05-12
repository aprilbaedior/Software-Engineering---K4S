using System.ComponentModel.DataAnnotations;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2023_Group1_Sections
    {
        [Key]
        public string SectionID { get; set; }
        public string? ServiceID { get; set; }
        public Boolean IsOnSunday { get; set; }
        public Boolean IsOnMonday { get; set; }
        public Boolean IsOnTuesday { get; set; }
        public Boolean IsOnWednesday { get; set; }
        public Boolean IsOnThursday { get; set; }
        public Boolean IsOnFriday { get; set; }
        public Boolean IsOnSaturday { get; set; }
       
        public Byte? StartMonth { get; set; }
        public Byte? EndMonth { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public int? SectionYear { get; set; }
    }
}
