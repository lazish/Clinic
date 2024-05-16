using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Clinic.Models
{
    public class Pin
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public Boolean IsPinned { get; set; }
        [AllowNull]
        public DateTime? PinDate { get; set; }
    }
}
