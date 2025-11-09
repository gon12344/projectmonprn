using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string StudentCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        // Navigation properties
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
