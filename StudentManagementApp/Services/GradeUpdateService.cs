using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    /// <summary>
    /// Event arguments for when a grade is changed.
    /// </summary>
    public class GradeChangedEventArgs : EventArgs
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public decimal? NewGrade { get; set; }
        
        // *** NEW: Add property for the date ***
        public DateTime? NewGradedDate { get; set; } 
    }

    /// <summary>
    /// Service responsible for updating grades and notifying subscribers (Observers).
    /// This implements the Observer pattern via a C# event.
    /// </summary>
    public class GradeUpdateService
    {
        private readonly DBContext _context;

        /// <summary>
        // This is the DELEGATE (Action<...>) and the EVENT.
        /// Observers (like ViewModels) can subscribe to this event.
        /// </summary>
        public event Action<object, GradeChangedEventArgs>? GradeChanged;

        public GradeUpdateService(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates a grade in the database and notifies all subscribers.
        /// </summary>
        public async Task<bool> UpdateGradeAsync(int enrollmentId, decimal? newGrade)
        {
            try
            {
                var enrollment = await _context.Enrollments.FindAsync(enrollmentId);
                if (enrollment == null)
                {
                    MessageBox.Show("Enrollment not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                enrollment.Grade = newGrade;
                enrollment.GradedDate = newGrade.HasValue ? DateTime.Now : null; // Set date if graded, clear if grade is removed

                await _context.SaveChangesAsync();

                // Fire the event to notify all observers
                GradeChanged?.Invoke(this, new GradeChangedEventArgs
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    CourseId = enrollment.CourseId,
                    NewGrade = enrollment.Grade,
                    NewGradedDate = enrollment.GradedDate // *** NEW: Pass the date back ***
                });

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating grade: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}