﻿using System.ComponentModel.DataAnnotations;

namespace Clinic.Helper
{
    public class EmailConfirm
    {
        internal bool Confirmed;

        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public DateTime ValidDate { get; set; }

    }
}
