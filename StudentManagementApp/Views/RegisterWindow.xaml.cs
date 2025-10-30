using System.Windows;
using WpfApp1.Models;
using WpfApp1.Data;
using WpfApp1.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;


namespace WpfApp1.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string fullName = FullNameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PwdBox.Password.Trim();
            var dob = DOBPicker.SelectedDate;
            string gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string phone = PhoneBox.Text.Trim();
            string address = AddrBox.Text.Trim();
            // Email và full name đã có bên trên

            if (string.IsNullOrWhiteSpace(username) 
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(fullName)
                || dob == null
                || string.IsNullOrWhiteSpace(gender)
                || string.IsNullOrWhiteSpace(phone)
                || string.IsNullOrWhiteSpace(address)
                || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("All fields are required. Please fill in all information!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var ctx = App.DbContext!;
            if (await ctx.Users.AnyAsync(u => u.Username == username))
            {
                MessageBox.Show("Username already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Create Role if needed
            var studentRole = ctx.Roles.FirstOrDefault(r => r.Name == "Student");
            int studentRoleId;
if (studentRole != null)
{
    studentRoleId = studentRole.RoleId;
}
else
{
    ctx.Roles.Add(new Role { Name = "Student" });
    ctx.SaveChanges();
    studentRoleId = ctx.Roles.First(r => r.Name == "Student").RoleId;
}
            // Tạo Student record kèm (code tự sinh : 'Auto'+UserId, fullName)
            var student = new Student
            {
                FullName = fullName,
                StudentCode = username.ToUpper(),
                Gender = gender,
                DateOfBirth = dob.Value,
                Address = address,
                PhoneNumber = phone,
                Email = email
            };
            ctx.Students.Add(student); await ctx.SaveChangesAsync();

            var user = new User
            {
                Username = username,
                PasswordHash = AuthService.ComputeSha256(password),
                FullName = fullName,
                Email = email,
                IsActive = true,
                RoleId = studentRoleId,
                StudentId = student.StudentId
            };
            ctx.Users.Add(user); await ctx.SaveChangesAsync();
            MessageBox.Show("Registration successful! You can login now.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
