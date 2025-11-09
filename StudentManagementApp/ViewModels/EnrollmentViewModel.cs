using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public partial class EnrollmentViewModel : BaseViewModel
    {
        private readonly DBContext _context;
        private readonly bool _isAdmin;

        [ObservableProperty]
        private ObservableCollection<Student> _students = new();

        [ObservableProperty]
        private ObservableCollection<Course> _courses = new();

        [ObservableProperty]
        private ObservableCollection<Enrollment> _enrollments = new();

        [ObservableProperty]
        private Student? _selectedStudent;

        [ObservableProperty]
        private Course? _selectedCourse;

        [ObservableProperty]
        private string _searchStudentText = string.Empty;

        [ObservableProperty]
        private string _searchCourseText = string.Empty;

        [ObservableProperty]
        private bool _isAdminUi;

        [ObservableProperty]
        private Enrollment? _selectedEnrollment;

        [ObservableProperty]
        private DateTime _enrollmentDate = DateTime.Now;

        [ObservableProperty]
        private string _enrollmentStatus = "Pending";

        [ObservableProperty]
        private string _enrollmentNotes = string.Empty;

        public EnrollmentViewModel(DBContext context)
        {
            _context = context;
            Title = "Enrollment Management";
            _isAdmin = string.Equals(App.CurrentUser?.Role?.Name, "Admin", StringComparison.OrdinalIgnoreCase);
            IsAdminUi = _isAdmin;
            // Defer async work to InitializeAsync to avoid concurrent DbContext operations
        }

        public async Task InitializeAsync()
        {
            await LoadData();
            if (!_isAdmin && App.CurrentUser?.StudentId != null)
            {
                var sid = App.CurrentUser.StudentId.Value;
                await PreselectStudentAsync(sid);
            }
        }

        private async Task PreselectStudentAsync(int studentId)
        {
            try
            {
                var stu = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
                if (stu != null)
                {
                    SelectedStudent = stu;
                    await LoadEnrollmentsForStudentInternal(studentId);
                }
            }
            catch { }
        }

        [RelayCommand]
        private async Task LoadData()
        {
            try
            {
                IsBusy = true;
                var students = await _context.Students.ToListAsync();
                var courses = await _context.Courses.Where(c => c.IsActive).ToListAsync();

                Students = new ObservableCollection<Student>(students);
                Courses = new ObservableCollection<Course>(courses);

                if (SelectedStudent != null)
                {
                    await LoadEnrollmentsForStudentInternal(SelectedStudent.StudentId);
                }
                else
                {
                    Enrollments.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadEnrollmentsForStudentInternal(int studentId)
        {
            var list = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
            Enrollments = new ObservableCollection<Enrollment>(list);
        }

        public async Task LoadEnrollmentsForStudent(int studentId)
        {
            await LoadEnrollmentsForStudentInternal(studentId);
        }

        [RelayCommand]
        private async Task Enroll()
        {
            if (SelectedStudent == null || SelectedCourse == null)
            {
                MessageBox.Show("Select a student and a course first.", "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var exists = await _context.Enrollments.AnyAsync(e => e.StudentId == SelectedStudent.StudentId && e.CourseId == SelectedCourse.CourseId);
            if (exists)
            {
                MessageBox.Show("This student has already enrolled in the course.", "Duplicate enrollment", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var enrollment = new Enrollment
            {
                StudentId = SelectedStudent.StudentId,
                CourseId = SelectedCourse.CourseId,
                Status = _isAdmin ? "Active" : "Pending",
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            await LoadEnrollmentsForStudentInternal(SelectedStudent.StudentId);
            MessageBox.Show(_isAdmin ? "Enrolled successfully!" : "Request submitted and waiting for approval.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private async Task Drop()
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Select a student.", "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = Enrollments.FirstOrDefault();
            if (selected == null)
            {
                MessageBox.Show("Select an enrollment record in the list.", "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _context.Enrollments.Remove(selected);
            await _context.SaveChangesAsync();
            await LoadEnrollmentsForStudentInternal(SelectedStudent.StudentId);
        }

        [RelayCommand]
        private async Task Approve()
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Only admin can approve.", "Forbidden", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selected = Enrollments.FirstOrDefault();
            if (selected == null)
            {
                MessageBox.Show("Select an enrollment record in the list.", "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            selected.Status = "Active";
            await _context.SaveChangesAsync();
            await LoadEnrollmentsForStudentInternal(SelectedStudent!.StudentId);
        }

        [RelayCommand]
        private async Task Reject()
        {
            if (!_isAdmin)
            {
                MessageBox.Show("Only admin can reject.", "Forbidden", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selected = Enrollments.FirstOrDefault();
            if (selected == null)
            {
                MessageBox.Show("Select an enrollment record in the list.", "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            selected.Status = "Rejected";
            await _context.SaveChangesAsync();
            await LoadEnrollmentsForStudentInternal(SelectedStudent!.StudentId);
        }

        [RelayCommand]
        private async Task SearchStudents()
        {
            IsBusy = true;
            try
            {
                var query = _context.Students.AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchStudentText))
                {
                    query = query.Where(s => s.FullName.Contains(SearchStudentText) || s.StudentCode.Contains(SearchStudentText));
                }
                Students = new ObservableCollection<Student>(await query.ToListAsync());
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SearchCourses()
        {
            IsBusy = true;
            try
            {
                var query = _context.Courses.AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchCourseText))
                {
                    query = query.Where(c => c.CourseName.Contains(SearchCourseText) || c.CourseCode.Contains(SearchCourseText));
                }
                Courses = new ObservableCollection<Course>(await query.ToListAsync());
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedStudentChanged(Student? value)
        {
            if (value != null)
            {
                LoadEnrollmentsForStudentInternal(value.StudentId).ConfigureAwait(false);
            }
        }
    }
}
