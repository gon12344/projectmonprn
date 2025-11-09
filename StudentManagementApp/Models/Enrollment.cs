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
        
        // *** NEW PROPERTIES FOR GRADING ***
        /// <summary>
        /// The grade received by the student (e.g., 8.5)
        /// Nullable if not graded yet.
        /// </summary>
        public decimal? Grade { get; set; }

        /// <summary>
        /// The date the grade was assigned.
        /// </summary>
        public DateTime? GradedDate { get; set; }
        // *** END NEW PROPERTIES ***
        
        [StringLength(200)]
        public string? Notes { get; set; }
    }
}
