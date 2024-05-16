using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class DoctorExperience
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public string Experience { get; set; }
    }
}
