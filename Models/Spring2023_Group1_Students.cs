using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CS395SI_Spring2023_K4S.Model
{
    public class Spring2023_Group1_Students
    {
        public string Name { get; set; }
        [Key]
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
