using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class StudentManagementView : UserControl
    {
        public StudentManagementView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is StudentManagementViewModel vm && vm.SelectedStudent != null)
            {
                // Load data into ViewModel properties
                vm.InputStudentCode = vm.SelectedStudent.StudentCode;
                vm.InputFullName = vm.SelectedStudent.FullName;
                vm.InputEmail = vm.SelectedStudent.Email;
                vm.InputPhoneNumber = vm.SelectedStudent.PhoneNumber;
                vm.InputAddress = vm.SelectedStudent.Address;
                vm.InputDateOfBirth = vm.SelectedStudent.DateOfBirth;
                vm.InputGender = vm.SelectedStudent.Gender switch
                {
                    "Nam" => "Male",
                    "Nu" => "Female",
                    _ => "Other"
                };
            }
        }
    }
}