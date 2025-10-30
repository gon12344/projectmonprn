using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int Credits { get; set; }
        
        [StringLength(50)]
        public string? Department { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
