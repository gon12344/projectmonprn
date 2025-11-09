using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class EnrollmentView : UserControl
 {
        private EnrollmentViewModel? _vm;
        
    public EnrollmentView()
   {
     InitializeComponent();
 
     // Set default values for form controls
      var enrollmentDatePicker = FindName("EnrollmentDatePicker") as DatePicker;
     var statusComboBox = FindName("StatusComboBox") as ComboBox;
      var notesBox = FindName("NotesBox") as TextBox;
  
      enrollmentDatePicker?.SetValue(DatePicker.SelectedDateProperty, DateTime.Now);
      statusComboBox?.SetValue(ComboBox.SelectedIndexProperty, 0);
      notesBox?.SetValue(TextBox.TextProperty, "Dang ky moi");
      
   if (App.DbContext != null)
 {
    _vm = new EnrollmentViewModel(App.DbContext);
 DataContext = _vm;
     this.Loaded += async (_, __) =>
    {
  if (_vm != null)
    {
       await _vm.InitializeAsync();
   }
   };
}
        }

      private async void SaveEnrollmentDetails_Click(object sender, RoutedEventArgs e)
 {
      try
    {
   if (_vm == null) return;

  // Validate required fields
    if (_vm.SelectedStudent == null)
    {
       MessageBox.Show("Vui long chon sinh vien!", "Thieu thong tin", 
 MessageBoxButton.OK, MessageBoxImage.Warning);
 return;
   }

       if (_vm.SelectedCourse == null)
 {
      MessageBox.Show("Vui long chon lop hoc!", "Thieu thong tin", 
     MessageBoxButton.OK, MessageBoxImage.Warning);
  return;
   }

 // Check if enrollment already exists
 var exists = await App.DbContext.Enrollments.AnyAsync(e => 
    e.StudentId == _vm.SelectedStudent.StudentId && 
 e.CourseId == _vm.SelectedCourse.CourseId);

        if (exists)
    {
     MessageBox.Show("Sinh vien da dang ky lop hoc nay roi!", "Trung lap", 
  MessageBoxButton.OK, MessageBoxImage.Information);
    return;
  }

  // Get values from form controls
 var enrollmentDatePicker = FindName("EnrollmentDatePicker") as DatePicker;
       var statusComboBox = FindName("StatusComboBox") as ComboBox;
 var notesBox = FindName("NotesBox") as TextBox;
  
  var enrollmentDate = enrollmentDatePicker?.SelectedDate ?? DateTime.Now;
     var status = ((ComboBoxItem?)statusComboBox?.SelectedItem)?.Content?.ToString() ?? "Pending";
       var notes = notesBox?.Text.Trim() ?? string.Empty;

     // Create new enrollment
      var newEnrollment = new Enrollment
          {
       StudentId = _vm.SelectedStudent.StudentId,
    CourseId = _vm.SelectedCourse.CourseId,
    EnrollmentDate = enrollmentDate,
   Status = status,
  Notes = notes
     };

  // Add to database
  App.DbContext.Enrollments.Add(newEnrollment);
  await App.DbContext.SaveChangesAsync();

     // Refresh the list
 await _vm.LoadEnrollmentsForStudent(_vm.SelectedStudent.StudentId);

    MessageBox.Show("Luu thong tin dang ky thanh cong!", "Thanh cong", 
   MessageBoxButton.OK, MessageBoxImage.Information);

 // Clear form
       ClearForm_Click(sender, e);
   }
     catch (Exception ex)
   {
   MessageBox.Show($"Loi khi luu: {ex.Message}", "Loi", 
   MessageBoxButton.OK, MessageBoxImage.Error);
  }
        }

  private void ClearForm_Click(object sender, RoutedEventArgs e)
  {
 // Reset form controls
  var enrollmentDatePicker = FindName("EnrollmentDatePicker") as DatePicker;
     var statusComboBox = FindName("StatusComboBox") as ComboBox;
   var notesBox = FindName("NotesBox") as TextBox;
      
        enrollmentDatePicker?.SetValue(DatePicker.SelectedDateProperty, DateTime.Now);
        statusComboBox?.SetValue(ComboBox.SelectedIndexProperty, 0);
   notesBox?.SetValue(TextBox.TextProperty, string.Empty);
      }
    }
}
