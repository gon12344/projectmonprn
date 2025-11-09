using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;
using System.Windows;

namespace WpfApp1.ViewModels
{
    public partial class CourseDetailViewModel : BaseViewModel
    {
        private readonly DBContext _context;
        private readonly bool _isEditMode;
        
        [ObservableProperty]
     private Course _course;

        [ObservableProperty]
        private string _validationMessage = string.Empty;

        public bool HasValidationMessage => !string.IsNullOrEmpty(ValidationMessage);

      public CourseDetailViewModel(DBContext context, Course course)
        {
    _context = context;
     _course = course;
       _isEditMode = course.CourseId > 0;
   Title = _isEditMode ? "Sua lop hoc" : "Them lop hoc moi";
 
     if (!_isEditMode)
          {
    Course.CreatedDate = DateTime.Now;
      Course.IsActive = true;
  }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (!ValidateCourse())
   return;

      try
            {
     IsBusy = true;
   
if (_isEditMode)
      {
     // Update existing course
            var existingCourse = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseId == Course.CourseId);
  
    if (existingCourse != null)
  {
           existingCourse.CourseCode = Course.CourseCode;
            existingCourse.CourseName = Course.CourseName;
                 existingCourse.Description = Course.Description;
          existingCourse.Credits = Course.Credits;
existingCourse.Department = Course.Department;
   existingCourse.IsActive = Course.IsActive;
   }
 }
              else
      {
         // Add new course
          _context.Courses.Add(Course);
       }
       
              await _context.SaveChangesAsync();
       
          MessageBox.Show(_isEditMode ? "Cap nhat lop hoc thanh cong!" : "Them lop hoc thanh cong!", 
   "Thong bao", MessageBoxButton.OK, MessageBoxImage.Information);
           
    CloseWindow(true);
          }
            catch (Exception ex)
      {
         ValidationMessage = $"Loi khi luu lop hoc: {ex.Message}";
     }
        finally
 {
           IsBusy = false;
    }
   }

       partial void OnValidationMessageChanged(string value)
  {
 OnPropertyChanged(nameof(HasValidationMessage));
  }

        [RelayCommand]
        private void Cancel()
        {
 CloseWindow(false);
        }

  private bool ValidateCourse()
        {
            ValidationMessage = string.Empty;
         var errors = new List<string>();

    // Required fields validation
 if (string.IsNullOrWhiteSpace(Course.CourseCode))
             errors.Add("Ma lop hoc khong duoc de trong.");
        
            if (string.IsNullOrWhiteSpace(Course.CourseName))
        errors.Add("Ten lop hoc khong duoc de trong.");

            // Credits validation
  if (Course.Credits <= 0 || Course.Credits > 20)
          errors.Add("So tin chi phai tu 1-20.");

            // Check duplicate course code
         if (!string.IsNullOrWhiteSpace(Course.CourseCode))
      {
         var isDuplicate = _context.Courses.Any(c => 
     c.CourseCode == Course.CourseCode && c.CourseId != Course.CourseId);
    
    if (isDuplicate)
  errors.Add("Ma lop hoc da ton tai.");
            }

            if (errors.Any())
          {
        ValidationMessage = string.Join("\n", errors);
         return false;
    }

  return true;
        }

        private void CloseWindow(bool dialogResult)
      {
  if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
  {
    window.DialogResult = dialogResult;
     window.Close();
    }
   }
    }
}