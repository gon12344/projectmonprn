using System;
using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        // Optional link to Student profile for Student role
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
