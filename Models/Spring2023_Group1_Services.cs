using System.ComponentModel.DataAnnotations;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2023_Group1_Services
    {
        [Display(Name = "Service")]
        public string? ServiceName { get; set; }
        [Key]
        public string ServiceID { get; set; }
        [Display(Name = "Frequency")]
        public string? ServiceFrequency { get; set; }
        [Display(Name = "Description")]
        public string? ServiceDescription { get; set; }
        [Display(Name = "Number of Hours")]
        public int ServiceHours { get; set; }
    }
}
