using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class CourseManagementView : UserControl
    {
        public CourseManagementView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is CourseManagementViewModel vm && vm.SelectedCourse != null)
            {
                // Load data into ViewModel properties
                vm.InputCourseCode = vm.SelectedCourse.CourseCode;
                vm.InputCourseName = vm.SelectedCourse.CourseName;
                vm.InputCredits = vm.SelectedCourse.Credits;
                vm.InputDepartment = vm.SelectedCourse.Department ?? string.Empty;
                vm.InputDescription = vm.SelectedCourse.Description ?? string.Empty;
                vm.InputIsActive = vm.SelectedCourse.IsActive;
            }
        }
    }
}