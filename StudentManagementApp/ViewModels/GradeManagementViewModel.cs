using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels
{
    public partial class GradeManagementViewModel : BaseViewModel
    {
        private readonly DBContext _context;
        private readonly GradeUpdateService _gradeUpdateService;

        [ObservableProperty]
        private ObservableCollection<Course> _courses = new();

        [ObservableProperty]
        private Course? _selectedCourse;

        [ObservableProperty]
        private ObservableCollection<Enrollment> _enrolledStudents = new();

        [ObservableProperty]
        private Enrollment? _selectedEnrollment;

        [ObservableProperty]
        private decimal? _currentGrade;

        // Statistics Properties
        [ObservableProperty]
        private string _courseAverage = "N/A";

        [ObservableProperty]
        private string _courseMax = "N/A";

        [ObservableProperty]
        private string _courseMin = "N/A";

        public GradeManagementViewModel(DBContext context, GradeUpdateService gradeUpdateService)
        {
            _context = context;
            _gradeUpdateService = gradeUpdateService;
            Title = "Grade Management";
            
            // !!! XÓA BỎ VIỆC GỌI LỆNH TẢI Ở ĐÂY !!!
            // LoadCoursesCommand.Execute(null); // <-- This is incorrect for async loading

            // Observer pattern: subscribe to grade change event
            _gradeUpdateService.GradeChanged += OnGradeChanged_UpdateStats;
        }

        // *** NEW: Method to be called by the View when it's loaded ***
        public async Task InitializeAsync()
        {
            await LoadCourses();
        }

        [RelayCommand]
        private async Task LoadCourses()
        {
            try
            {
                IsBusy = true;
                var courses = await _context.Courses.Where(c => c.IsActive).ToListAsync();
                Courses = new ObservableCollection<Course>(courses);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading courses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedCourseChanged(Course? value)
        {
            if (value != null)
            {
                // This is a good way to call async in a property change
                _ = LoadEnrollmentsForCourse(value.CourseId); 
            }
            else
            {
                EnrolledStudents.Clear();
                RecalculateStatistics();
            }
        }

        partial void OnSelectedEnrollmentChanged(Enrollment? value)
        {
            // This is perfect
            CurrentGrade = value?.Grade;
        }

        private async Task LoadEnrollmentsForCourse(int courseId)
        {
            try
            {
                IsBusy = true;
                var enrollments = await _context.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.CourseId == courseId)
                    .OrderBy(e => e.Student!.FullName)
                    .ToListAsync();

                EnrolledStudents = new ObservableCollection<Enrollment>(enrollments);
                RecalculateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading enrollments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateGrade()
        {
            if (SelectedEnrollment == null)
            {
                MessageBox.Show("Please select a student first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentGrade.HasValue && (CurrentGrade < 0 || CurrentGrade > 10))
            {
                MessageBox.Show("Grade must be between 0 and 10.", "Invalid Grade", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsBusy = true;
            await _gradeUpdateService.UpdateGradeAsync(SelectedEnrollment.EnrollmentId, CurrentGrade);
            IsBusy = false;
        }

        // This method will now compile and work correctly
        private void OnGradeChanged_UpdateStats(object? sender, GradeChangedEventArgs e)
        {
            if (SelectedCourse != null && e.CourseId == SelectedCourse.CourseId)
            {
                var enrollmentInList = EnrolledStudents.FirstOrDefault(en => en.EnrollmentId == e.EnrollmentId);
                if (enrollmentInList != null)
                {
                    // Update local list from event data
                    enrollmentInList.Grade = e.NewGrade;
                    enrollmentInList.GradedDate = e.NewGradedDate; // <-- This now works!

                    if (SelectedEnrollment?.EnrollmentId == e.EnrollmentId)
                    {
                        CurrentGrade = e.NewGrade;
                    }
                    
                    // Refresh the DataGrid
                    EnrolledStudents = new ObservableCollection<Enrollment>(EnrolledStudents);
                }

                RecalculateStatistics();
            }
        }

        private void RecalculateStatistics()
        {
            if (EnrolledStudents == null || !EnrolledStudents.Any())
            {
                CourseAverage = "N/A";
                CourseMax = "N/A";
                CourseMin = "N/A";
                return;
            }

            var graded = EnrolledStudents.Where(e => e.Grade.HasValue).Select(e => e.Grade!.Value).ToList();
            if (!graded.Any())
            {
                CourseAverage = "0.0";
                CourseMax = "N/A";
                CourseMin = "N/A";
                return;
            }

            CourseAverage = $"{graded.Average():F1}";
            CourseMax = $"{graded.Max():F1}";
            CourseMin = $"{graded.Min():F1}";
        }
    }
}