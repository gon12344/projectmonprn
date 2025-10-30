using System;
using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Completed, Dropped
        
        
        
        [StringLength(200)]
        public string? Notes { get; set; }
    }
}
