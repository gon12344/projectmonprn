using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Views;
using WpfApp1.Services;
using System.Windows;

namespace WpfApp1.ViewModels
{
 public partial class StudentManagementViewModel : BaseViewModel
    {
        private readonly DBContext _context;
      private readonly StudentCollectionService _collectionService;
        
 [ObservableProperty]
      private ICollectionView _filteredStudents;
        
   [ObservableProperty]
        private Student? _selectedStudent;
        
     [ObservableProperty]
        private string _searchText = string.Empty;

        // Form input properties
  [ObservableProperty]
        private string _inputStudentCode = string.Empty;

  [ObservableProperty]
  private string _inputFullName = string.Empty;

        [ObservableProperty]
  private string _inputEmail = string.Empty;

  [ObservableProperty]
private string _inputPhoneNumber = string.Empty;

  [ObservableProperty]
     private string _inputAddress = string.Empty;

        [ObservableProperty]
  private DateTime? _inputDateOfBirth = null;

        [ObservableProperty]
  private string _inputGender = "Male";

        // S? d?ng Collection Generics thông qua service
   public ObservableCollection<Student> StudentsCollection => _collectionService.Students;

        // Expose LoadStudentsCommand cho View  
   public IAsyncRelayCommand LoadStudentsAsyncCommand => LoadStudentsCommand;

     public StudentManagementViewModel(DBContext context, StudentCollectionService collectionService)
 {
    _context = context;
    _collectionService = collectionService;
    Title = "Quan ly sinh vien";
            
        // T?o CollectionView ?? filter t? Collection Generic
        FilteredStudents = CollectionViewSource.GetDefaultView(StudentsCollection);
     FilteredStudents.Filter = FilterStudents;
          
      LoadStudents();
        }

        private bool FilterStudents(object obj)
        {
      if (obj is not Student student) return false;
        if (string.IsNullOrWhiteSpace(SearchText)) return true;
       
        // S? d?ng Collection Generic service cho tìm ki?m
         var searchResults = _collectionService.SearchStudents(SearchText);
      return searchResults.Contains(student);
        }

   partial void OnSearchTextChanged(string value)
        {
     FilteredStudents.Refresh();
}

     [RelayCommand]
     private async Task LoadStudents()
      {
   try
        {
   IsBusy = true;
     var students = await _context.Students
     .AsNoTracking()
   .OrderBy(s => s.StudentCode)
   .ToListAsync();
 
       // S? d?ng Collection Generics bulk load
      _collectionService.LoadStudentsFromCollection(students);
      }
        catch (Exception ex)
        {
    MessageBox.Show($"Loi khi tai danh sach sinh vien: {ex.Message}", "Loi", 
MessageBoxButton.OK, MessageBoxImage.Error);
      }
 finally
  {
    IsBusy = false;
        }
        }

   [RelayCommand]
  private async Task AddStudentAsync()
   {
        // Check if form has data
  if (!string.IsNullOrWhiteSpace(InputStudentCode) && !string.IsNullOrWhiteSpace(InputFullName))
 {
      // Use form data
   if (!ValidateFormInput())
    return;

      try
        {
      IsBusy = true;

  // Check duplicate
    if (_context.Students.Any(s => s.StudentCode == InputStudentCode.Trim()))
 {
  MessageBox.Show("Student ID already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
     return;
   }

      var newStudent = new Student
     {
     StudentCode = InputStudentCode.Trim(),
      FullName = InputFullName.Trim(),
     Email = InputEmail.Trim(),
     PhoneNumber = InputPhoneNumber.Trim(),
  Address = InputAddress.Trim(),
    DateOfBirth = InputDateOfBirth!.Value,
  Gender = InputGender
   };

   _context.Students.Add(newStudent);
 await _context.SaveChangesAsync();

  MessageBox.Show("Student added successfully!", "Success", 
    MessageBoxButton.OK, MessageBoxImage.Information);

   ClearFormInput();
      await LoadStudents();
   }
catch (Exception ex)
    {
    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
 }
   finally
   {
     IsBusy = false;
  }
     }
  else
   {
      // Use detail window if no form data
  var detailViewModel = new StudentDetailViewModel(_context, new Student());
   var detailWindow = new StudentDetailWindow
{
     DataContext = detailViewModel
   };

 if (detailWindow.ShowDialog() == true)
 {
     await LoadStudents();
  }
   }
  }

  [RelayCommand(CanExecute = nameof(CanEditOrDeleteStudent))]
   private async Task EditStudentAsync()
  {
  if (SelectedStudent == null) return;

        // Check if form has data - use form
     if (!string.IsNullOrWhiteSpace(InputStudentCode) && !string.IsNullOrWhiteSpace(InputFullName))
   {
        if (!ValidateFormInput())
   return;

   try
      {
     IsBusy = true;

   var student = await _context.Students.FindAsync(SelectedStudent.StudentId);
     if (student != null)
    {
   student.FullName = InputFullName.Trim();
     student.Email = InputEmail.Trim();
   student.PhoneNumber = InputPhoneNumber.Trim();
 student.Address = InputAddress.Trim();
      student.DateOfBirth = InputDateOfBirth!.Value;
     student.Gender = InputGender;

    await _context.SaveChangesAsync();

      MessageBox.Show("Student updated successfully!", "Success", 
     MessageBoxButton.OK, MessageBoxImage.Information);

        ClearFormInput();
  await LoadStudents();
     }
 }
        catch (Exception ex)
 {
   MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
   finally
        {
   IsBusy = false;
}
   }
  else
   {
      // Use detail window (old approach)
  var studentToEdit = new Student
 {
  StudentId = SelectedStudent.StudentId,
   StudentCode = SelectedStudent.StudentCode,
  FullName = SelectedStudent.FullName,
 Email = SelectedStudent.Email,
      PhoneNumber = SelectedStudent.PhoneNumber,
     DateOfBirth = SelectedStudent.DateOfBirth,
 Gender = SelectedStudent.Gender,
   Address = SelectedStudent.Address
     };
  
 var detailViewModel = new StudentDetailViewModel(_context, studentToEdit);
     var detailWindow = new StudentDetailWindow
     {
 DataContext = detailViewModel
   };

  if (detailWindow.ShowDialog() == true)
 {
   await LoadStudents();
   }
   }
    }

  private bool CanEditOrDeleteStudent() => SelectedStudent != null;

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteStudent))]
  private async Task DeleteStudent()
  {
   if (SelectedStudent == null) return;

      var result = MessageBox.Show(
 $"Are you sure you want to delete student '{SelectedStudent.FullName}'?",
    "Confirm Delete",
 MessageBoxButton.YesNo,
    MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

  try
 {
    IsBusy = true;

   // Check if student has enrollments
 var hasEnrollments = await _context.Enrollments
 .AnyAsync(e => e.StudentId == SelectedStudent.StudentId);

   if (hasEnrollments)
 {
   MessageBox.Show("Cannot delete student because they have course enrollments.", "Error",
     MessageBoxButton.OK, MessageBoxImage.Warning);
return;
   }

      var studentToDelete = await _context.Students
 .FirstOrDefaultAsync(s => s.StudentId == SelectedStudent.StudentId);

      if (studentToDelete != null)
  {
     _context.Students.Remove(studentToDelete);
   await _context.SaveChangesAsync();

   _collectionService.RemoveStudent(SelectedStudent);
   SelectedStudent = null;

    MessageBox.Show("Student deleted successfully!", "Success",
     MessageBoxButton.OK, MessageBoxImage.Information);
   }
  }
     catch (Exception ex)
  {
 MessageBox.Show($"Error deleting student: {ex.Message}", "Error",
   MessageBoxButton.OK, MessageBoxImage.Error);
    }
 finally
 {
      IsBusy = false;
}
        }

  [RelayCommand]
 private async Task Refresh()
    {
 await LoadStudents();
 }

  [RelayCommand]
  private void Search()
    {
   FilteredStudents.Refresh();
        }

   private bool ValidateFormInput()
 {
   if (string.IsNullOrWhiteSpace(InputStudentCode))
   {
     MessageBox.Show("Please enter student ID!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
      return false;
   }

   if (string.IsNullOrWhiteSpace(InputFullName))
  {
  MessageBox.Show("Please enter full name!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    return false;
  }

   if (string.IsNullOrWhiteSpace(InputEmail))
 {
   MessageBox.Show("Please enter email!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
return false;
 }

   if (string.IsNullOrWhiteSpace(InputPhoneNumber))
{
  MessageBox.Show("Please enter phone number!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
   return false;
 }

   if (string.IsNullOrWhiteSpace(InputAddress))
      {
     MessageBox.Show("Please enter address!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
  return false;
 }

   if (InputDateOfBirth == null)
  {
    MessageBox.Show("Please select date of birth!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
  return false;
  }

        if (InputDateOfBirth.Value > DateTime.Now.AddYears(-15))
   {
  MessageBox.Show("Student must be at least 15 years old!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
  return false;
   }

  return true;
      }

  private void ClearFormInput()
  {
   InputStudentCode = string.Empty;
  InputFullName = string.Empty;
     InputEmail = string.Empty;
  InputPhoneNumber = string.Empty;
   InputAddress = string.Empty;
      InputDateOfBirth = null;
 InputGender = "Male";
     SelectedStudent = null;
 }
    }
}