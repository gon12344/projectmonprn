using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.Data;
using System.Linq;

namespace WpfApp1.Views
{
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
            if (App.CurrentUser != null)
            {
                // Nếu là student, ưu tiên tên từ đối tượng Student liên kết
                var ctx = App.DbContext;
                string role = App.CurrentUser.Role?.Name ?? "";
                string fullname = App.CurrentUser.FullName ?? "";
                if (string.Equals(role, "Student", System.StringComparison.OrdinalIgnoreCase)
                    && App.CurrentUser.StudentId != null && ctx != null)
                {
                    var stu = ctx.Students.FirstOrDefault(s => s.StudentId == App.CurrentUser.StudentId);
                    if (stu != null) fullname = stu.FullName;
                }
                DataContext = new ProfileVm(App.CurrentUser, fullname);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Close cửa sổ hiện tại và mở lại LoginWindow
            foreach (Window win in Application.Current.Windows)
            {
                if (win.Title.Contains("Student Management") || win.Title.Contains("Student Portal"))
                {
                    win.Close();
                    break;
                }
            }
            new LoginWindow().Show();
        }
    }

    public class ProfileVm
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }

        public ProfileVm(User user, string fullname)
        {
            Username = user.Username;
            FullName = fullname;
            Email = user.Email ?? "";
            RoleName = user.Role?.Name ?? "";
        }
    }
}
