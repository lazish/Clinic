using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Clinic.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string IdentityNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; } = "client";
        [AllowNull]
        public string? ImageUrl { get; set; }
        [AllowNull]
        public string? CV { get; set; }
        [AllowNull]
        public string? Token { get; set; }
        [AllowNull]
        public int? CategoryId { get; set; }
        [AllowNull]
        public bool? TwoFactory { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string GeneratedCode { get; set; }

        public bool IsVerified { get; set; }
    }
}
