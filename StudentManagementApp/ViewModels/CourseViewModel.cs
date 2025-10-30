using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Factories;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public partial class CourseViewModel : BaseViewModel
    {
        private readonly DBContext _context;
        private readonly ICourseFactory _courseFactory;

        [ObservableProperty]
        private ObservableCollection<Course> _courses = new();

        [ObservableProperty]
        private Course? _selectedCourse;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _courseCode = string.Empty;

        [ObservableProperty]
        private string _courseName = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private int _credits = 3;

        [ObservableProperty]
        private string _department = string.Empty;

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private bool _isEditMode = false;

        public CourseViewModel(DBContext context, ICourseFactory courseFactory)
        {
            _context = context;
            _courseFactory = courseFactory;
            Title = "Course Management";
            LoadCourses();
        }

        [RelayCommand]
        private async Task LoadCourses()
        {
            try
            {
                IsBusy = true;
                var courses = await _context.Courses.ToListAsync();
                Courses.Clear();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
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

        [RelayCommand]
        private void NewCourse()
        {
            ClearForm();
            IsEditMode = false;
        }

        [RelayCommand]
        private async Task SaveCourse()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CourseCode) || string.IsNullOrWhiteSpace(CourseName))
                {
                    MessageBox.Show("Please fill in required fields (Course Code and Course Name).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (IsEditMode && SelectedCourse != null)
                {
                    // Update existing course
                    SelectedCourse.CourseCode = CourseCode;
                    SelectedCourse.CourseName = CourseName;
                    SelectedCourse.Description = Description;
                    SelectedCourse.Credits = Credits;
                    SelectedCourse.Department = Department;
                    SelectedCourse.IsActive = IsActive;
                }
                else
                {
                    // Create new course using Factory Pattern
                    var newCourse = _courseFactory.CreateCourse(CourseCode, CourseName, Credits, Department, Description);
                    newCourse.IsActive = IsActive;
                    _context.Courses.Add(newCourse);
                }

                await _context.SaveChangesAsync();
                await LoadCourses();
                ClearForm();
                MessageBox.Show("Course saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void EditCourse()
        {
            if (SelectedCourse != null)
            {
                CourseCode = SelectedCourse.CourseCode;
                CourseName = SelectedCourse.CourseName;
                Description = SelectedCourse.Description ?? string.Empty;
                Credits = SelectedCourse.Credits;
                Department = SelectedCourse.Department ?? string.Empty;
                IsActive = SelectedCourse.IsActive;
                IsEditMode = true;
            }
        }

        [RelayCommand]
        private async Task DeleteCourse()
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please select a course to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete course '{SelectedCourse.CourseName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Courses.Remove(SelectedCourse);
                    await _context.SaveChangesAsync();
                    await LoadCourses();
                    ClearForm();
                    MessageBox.Show("Course deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task SearchCourses()
        {
            try
            {
                IsBusy = true;
                var query = _context.Courses.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(c => c.CourseName.Contains(SearchText) || 
                                           c.CourseCode.Contains(SearchText) || 
                                           c.Department!.Contains(SearchText));
                }

                var courses = await query.ToListAsync();
                Courses.Clear();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching courses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
            LoadCourses();
        }

        private void ClearForm()
        {
            CourseCode = string.Empty;
            CourseName = string.Empty;
            Description = string.Empty;
            Credits = 3;
            Department = string.Empty;
            IsActive = true;
            SelectedCourse = null;
            IsEditMode = false;
        }

        partial void OnSelectedCourseChanged(Course? value)
        {
            if (value != null)
            {
                CourseCode = value.CourseCode;
                CourseName = value.CourseName;
                Description = value.Description ?? string.Empty;
                Credits = value.Credits;
                Department = value.Department ?? string.Empty;
                IsActive = value.IsActive;
            }
        }
    }
}
