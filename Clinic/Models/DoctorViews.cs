using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class DoctorViews
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DoctorId { get; set; }
    }
}
