using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Clinic.Helper
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [AllowNull]
        public string? Description { get; set; }
    }
}