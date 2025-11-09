using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;
using System.Windows;

namespace WpfApp1.ViewModels
{
    public partial class StudentDetailViewModel : BaseViewModel
    {
        private readonly DBContext _context;
    private readonly bool _isEditMode;
        
     [ObservableProperty]
        private Student _student;
        
        [ObservableProperty]
        private string _validationMessage = string.Empty;

        public bool HasValidationMessage => !string.IsNullOrEmpty(ValidationMessage);

  public StudentDetailViewModel(DBContext context, Student student)
    {
        _context = context;
         _student = student;
         _isEditMode = student.StudentId > 0;
Title = _isEditMode ? "Sua sinh vien" : "Them sinh vien moi";
 }

        [RelayCommand]
        private async Task Save()
        {
  if (!ValidateStudent())
                return;

    try
      {
   IsBusy = true;
      
                if (_isEditMode)
       {
     // Update existing student
   var existingStudent = await _context.Students
         .FirstOrDefaultAsync(s => s.StudentId == Student.StudentId);
        
          if (existingStudent != null)
       {
   existingStudent.StudentCode = Student.StudentCode;
               existingStudent.FullName = Student.FullName;
       existingStudent.Email = Student.Email;
         existingStudent.PhoneNumber = Student.PhoneNumber;
      existingStudent.DateOfBirth = Student.DateOfBirth;
    existingStudent.Gender = Student.Gender;
        existingStudent.Address = Student.Address;
          }
       }
    else
     {
      // Add new student
          _context.Students.Add(Student);
         }
    
    await _context.SaveChangesAsync();
           
      MessageBox.Show(_isEditMode ? "Cap nhat sinh vien thanh cong!" : "Them sinh vien thanh cong!", 
        "Thong bao", MessageBoxButton.OK, MessageBoxImage.Information);
        
    CloseWindow(true);
    }
      catch (Exception ex)
{
         ValidationMessage = $"Loi khi luu sinh vien: {ex.Message}";
   }
          finally
         {
        IsBusy = false;
}
        }

        [RelayCommand]
        private void Cancel()
        {
 CloseWindow(false);
        }

partial void OnValidationMessageChanged(string value)
  {
 OnPropertyChanged(nameof(HasValidationMessage));
        }

  private bool ValidateStudent()
     {
    ValidationMessage = string.Empty;
  var errors = new List<string>();

  // Required fields validation
   if (string.IsNullOrWhiteSpace(Student.StudentCode))
        errors.Add("Ma sinh vien khong duoc de trong.");
       
      if (string.IsNullOrWhiteSpace(Student.FullName))
errors.Add("Ho va ten khong duoc de trong.");

     // Date validation
         if (Student.DateOfBirth == default || Student.DateOfBirth > DateTime.Now.AddYears(-15))
           errors.Add("Ngay sinh khong hop le (phai truoc 15 nam).");

    // Email validation
            if (!string.IsNullOrWhiteSpace(Student.Email))
            {
    var emailAttribute = new EmailAddressAttribute();
       if (!emailAttribute.IsValid(Student.Email))
          errors.Add("Email khong dung dinh dang.");
            }

            // Check duplicate student code
 if (!string.IsNullOrWhiteSpace(Student.StudentCode))
          {
     var isDuplicate = _context.Students.Any(s => 
   s.StudentCode == Student.StudentCode && s.StudentId != Student.StudentId);
        
                if (isDuplicate)
  errors.Add("Ma sinh vien da ton tai.");
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