using System.Windows;
using WpfApp1.Data;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string newpwd = NewPwdBox.Password.Trim();
            var ctx = App.DbContext!;
            var user = ctx.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                MessageBox.Show("Username not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            user.PasswordHash = AuthService.ComputeSha256(newpwd);
            await ctx.SaveChangesAsync();
            MessageBox.Show("Password reset successful. You can login now.", "Reset Password", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
