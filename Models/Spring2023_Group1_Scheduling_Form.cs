using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;


namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2023_Group1_Scheduling_Form
    {
        [Key]
        public int RequestID { get; set; }
        public string ServiceID { get; set; }
        public string Email { get; set; }
        [Display(Name = "Sunday")]
        public Boolean IsOnSunday { get; set; }
        [Display(Name = "Monday")]
        public Boolean IsOnMonday { get; set; }
        [Display(Name = "Tuesday")]
        public Boolean IsOnTuesday { get; set; }
        [Display(Name = "Wednesday")]
        public Boolean IsOnWednesday { get; set; }
        [Display(Name = "Thursday")]
        public Boolean IsOnThursday { get; set; }
        [Display(Name = "Friday")]
        public Boolean IsOnFriday { get; set; }
        [Display(Name = "Saturday")]
        public Boolean IsOnSaturday { get; set; }

        [Display(Name = "Start Month")]
        public string? StartMonth { get; set; }
        [Display(Name = "End Month")]
        public string? EndMonth { get; set; }
        [Display(Name = "Start Time")]
        public TimeSpan? StartTime { get; set; }
        [Display(Name = "End Time")]
        public TimeSpan? EndTime { get; set; }
        public string Status { get; set; } = "Pending"; // Pending
    }
}