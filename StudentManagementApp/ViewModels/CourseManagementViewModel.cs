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
   public partial class CourseManagementViewModel : BaseViewModel
    {
     private readonly DBContext _context;
        private readonly StudentCollectionService _collectionService;
        
 [ObservableProperty]
 private ICollectionView _filteredCourses;
        
      [ObservableProperty]
        private Course? _selectedCourse;
        
        [ObservableProperty]
        private string _searchText = string.Empty;

        // Form input properties
  [ObservableProperty]
        private string _inputCourseCode = string.Empty;

  [ObservableProperty]
private string _inputCourseName = string.Empty;

        [ObservableProperty]
  private int _inputCredits = 3;

  [ObservableProperty]
private string _inputDepartment = string.Empty;

  [ObservableProperty]
     private string _inputDescription = string.Empty;

        [ObservableProperty]
  private bool _inputIsActive = true;
        
     [ObservableProperty]
      private ObservableCollection<Enrollment> _studentsInSelectedCourse = new();

        // S? d?ng Collection Generics thông qua service
        public ObservableCollection<Course> CoursesCollection => _collectionService.Courses;
     public ObservableCollection<Enrollment> EnrollmentsCollection => _collectionService.Enrollments;

        public CourseManagementViewModel(DBContext context, StudentCollectionService collectionService)
        {
  _context = context;
     _collectionService = collectionService;
            Title = "Quan ly lop hoc";
  
         // T?o CollectionView ?? filter t? Collection Generic
         FilteredCourses = CollectionViewSource.GetDefaultView(CoursesCollection);
        FilteredCourses.Filter = FilterCourses;
        
      LoadCourses();
        }

        private bool FilterCourses(object obj)
     {
      if (obj is not Course course) return false;
    if (string.IsNullOrWhiteSpace(SearchText)) return true;
  
      // S? d?ng Collection Generic service cho tìm ki?m
      var searchResults = _collectionService.SearchCourses(SearchText);
     return searchResults.Contains(course);
     }
        
      partial void OnSearchTextChanged(string value)
     {
    FilteredCourses.Refresh();
        }

   partial void OnSelectedCourseChanged(Course? value)
        {
      LoadStudentsInCourse();
}

    [RelayCommand]
    private async Task LoadCourses()
        {
  try
      {
   IsBusy = true;
    var courses = await _context.Courses
    .AsNoTracking()
         .OrderBy(c => c.CourseCode)
       .ToListAsync();

     // S? d?ng Collection Generics bulk load
         _collectionService.LoadCoursesFromCollection(courses);
            
       // Load enrollments for statistics
  var enrollments = await _context.Enrollments
      .Include(e => e.Student)
          .Include(e => e.Course)
      .AsNoTracking()
      .ToListAsync();
     _collectionService.LoadEnrollmentsFromCollection(enrollments);
     }
       catch (Exception ex)
        {
 MessageBox.Show($"Error when loading course list: {ex.Message}", "Error", 
         MessageBoxButton.OK, MessageBoxImage.Error);
      }
        finally
 {
      IsBusy = false;
  }
    }

 [RelayCommand]
     private void LoadStudentsInCourse()
    {
      if (SelectedCourse == null)
    {
     StudentsInSelectedCourse.Clear();
   return;
        }

  try
     {
   // S? d?ng Collection Generic service
        var enrollments = _collectionService.GetEnrollmentsByCourseId(SelectedCourse.CourseId);
        
        StudentsInSelectedCourse.Clear();
     foreach (var enrollment in enrollments.OrderBy(e => e.Student?.StudentCode))
            {
        StudentsInSelectedCourse.Add(enrollment);
}
      }
        catch (Exception ex)
        {
MessageBox.Show($"Error when loading student list: {ex.Message}", "Error", 
      MessageBoxButton.OK, MessageBoxImage.Error);
 }
}

  [RelayCommand]
 private async Task AddCourseAsync()
 {
        // Check if form has data
  if (!string.IsNullOrWhiteSpace(InputCourseCode) && !string.IsNullOrWhiteSpace(InputCourseName))
 {
      // Use form data
   if (!ValidateCourseInput())
    return;

      try
    {
      IsBusy = true;

    // Check duplicate
    if (_context.Courses.Any(c => c.CourseCode == InputCourseCode.Trim()))
 {
  MessageBox.Show("Course code already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
     return;
   }

      var newCourse = new Course
   {
     CourseCode = InputCourseCode.Trim(),
      CourseName = InputCourseName.Trim(),
     Credits = InputCredits,
     Department = InputDepartment.Trim(),
      Description = InputDescription.Trim(),
    IsActive = InputIsActive,
  CreatedDate = DateTime.Now
};

   _context.Courses.Add(newCourse);
    await _context.SaveChangesAsync();

     MessageBox.Show("Course added successfully!", "Success", 
      MessageBoxButton.OK, MessageBoxImage.Information);

   ClearCourseInput();
      await LoadCourses();
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
  var detailViewModel = new CourseDetailViewModel(_context, new Course());
    var detailWindow = new CourseDetailWindow
 {
   DataContext = detailViewModel
     };

     if (detailWindow.ShowDialog() == true)
        {
  await LoadCourses();
    }
   }
}

    [RelayCommand(CanExecute = nameof(CanEditOrDeleteCourse))]
   private async Task EditCourseAsync()
   {
      if (SelectedCourse == null) return;

  // Check if form has data - use form
     if (!string.IsNullOrWhiteSpace(InputCourseCode) && !string.IsNullOrWhiteSpace(InputCourseName))
  {
  if (!ValidateCourseInput())
    return;

      try
  {
   IsBusy = true;

     var course = await _context.Courses.FindAsync(SelectedCourse.CourseId);
       if (course != null)
     {
     course.CourseName = InputCourseName.Trim();
     course.Credits = InputCredits;
  course.Department = InputDepartment.Trim();
    course.Description = InputDescription.Trim();
  course.IsActive = InputIsActive;

      await _context.SaveChangesAsync();

     MessageBox.Show("Course updated successfully!", "Success", 
 MessageBoxButton.OK, MessageBoxImage.Information);

ClearCourseInput();
     await LoadCourses();
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
   var courseToEdit = new Course
    {
    CourseId = SelectedCourse.CourseId,
      CourseCode = SelectedCourse.CourseCode,
    CourseName = SelectedCourse.CourseName,
      Description = SelectedCourse.Description,
 Credits = SelectedCourse.Credits,
  Department = SelectedCourse.Department,
    IsActive = SelectedCourse.IsActive,
     CreatedDate = SelectedCourse.CreatedDate
 };
   
        var detailViewModel = new CourseDetailViewModel(_context, courseToEdit);
        var detailWindow = new CourseDetailWindow
     {
DataContext = detailViewModel
       };

      if (detailWindow.ShowDialog() == true)
   {
   await LoadCourses();
  }
   }
  }

  private bool CanEditOrDeleteCourse() => SelectedCourse != null;

  [RelayCommand(CanExecute = nameof(CanEditOrDeleteCourse))]
      private async Task DeleteCourse()
 {
      if (SelectedCourse == null) return;

  var result = MessageBox.Show(
  $"Are you sure you want to delete course '{SelectedCourse.CourseName}'?",
      "Confirm Delete",
   MessageBoxButton.YesNo,
   MessageBoxImage.Question);

     if (result != MessageBoxResult.Yes) return;

        try
  {
   IsBusy = true;

 // Check if course has enrollments
      var hasEnrollments = await _context.Enrollments
   .AnyAsync(e => e.CourseId == SelectedCourse.CourseId);

 if (hasEnrollments)
   {
 MessageBox.Show("Cannot delete course because it has student enrollments.", "Error",
       MessageBoxButton.OK, MessageBoxImage.Warning);
     return;
  }

   var courseToDelete = await _context.Courses
    .FirstOrDefaultAsync(c => c.CourseId == SelectedCourse.CourseId);

  if (courseToDelete != null)
 {
      _context.Courses.Remove(courseToDelete);
 await _context.SaveChangesAsync();

        _collectionService.RemoveCourse(SelectedCourse);
        SelectedCourse = null;

   MessageBox.Show("Course deleted successfully!", "Success",
   MessageBoxButton.OK, MessageBoxImage.Information);
      }
     }
  catch (Exception ex)
   {
  MessageBox.Show($"Error deleting course: {ex.Message}", "Error",
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
   await LoadCourses();
    }

        [RelayCommand]
 private void Search()
        {
 FilteredCourses.Refresh();
  }

   private bool ValidateCourseInput()
 {
   if (string.IsNullOrWhiteSpace(InputCourseCode))
   {
     MessageBox.Show("Please enter course code!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
      return false;
   }

   if (string.IsNullOrWhiteSpace(InputCourseName))
  {
  MessageBox.Show("Please enter course name!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    return false;
  }

   if (InputCredits <= 0 || InputCredits > 20)
 {
   MessageBox.Show("Credits must be between 1-20!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
  return false;
   }

  return true;
      }

  private void ClearCourseInput()
  {
   InputCourseCode = string.Empty;
  InputCourseName = string.Empty;
     InputCredits = 3;
  InputDepartment = string.Empty;
   InputDescription = string.Empty;
      InputIsActive = true;
     SelectedCourse = null;
 }
  }
}