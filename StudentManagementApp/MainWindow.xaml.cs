using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Hide Courses tab for non-admin users
            var roleName = App.CurrentUser?.Role?.Name;
            if (!string.Equals(roleName, "Admin", System.StringComparison.OrdinalIgnoreCase))
            {
                if (CoursesTab != null)
                {
                    CoursesTab.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}