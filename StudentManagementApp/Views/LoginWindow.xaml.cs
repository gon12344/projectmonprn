using System.Windows;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public string Username { get; set; } = string.Empty;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
            _authService = new AuthService(App.DbContext!);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var password = PasswordBox.Password;
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter username and password.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var loginSuccess = await _authService.LoginAsync(Username, password);
                if (!loginSuccess)
                {
                    MessageBox.Show("Invalid credentials or inactive account.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Set current user globally
                App.CurrentUser = _authService.CurrentUser;

                // Navigate based on role
                if (string.Equals(App.CurrentUser?.Role?.Name, "Admin", System.StringComparison.OrdinalIgnoreCase))
                {
                    var main = new MainWindow();
                    main.Show();
                }
                else
                {
                    var studentWin = new StudentWindow();
                    studentWin.Show();
                }

                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }

        private void Forgot_Click(object sender, RoutedEventArgs e)
        {
            new ForgotPasswordWindow().ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
