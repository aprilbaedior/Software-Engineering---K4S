using System.ComponentModel.DataAnnotations;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2024_Group2_Sections
    {
        //There is no need for me to create another class when I can  just add capacity field to the existing Sections class.
        [Key]
        public int sectionID { get; set; }
        public string serviceID {  get; set; }
        public string? serviceName {  get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string weekDay { get; set; }
        public TimeSpan? startTime { get; set; }
        public TimeSpan? endTime { get; set; }
        public string status { get; set; }
        public int capacity { get; set; } //Added capacity field to track the maximum number of students allowed in the section
    }
}
