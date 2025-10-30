using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class StudentWindow : Window
    {
        public StudentWindow()
        {
            InitializeComponent();
            if (App.DbContext != null)
            {
                // EnrollmentView hosts its own DataContext, but set a window-level VM in case
                DataContext = new EnrollmentViewModel(App.DbContext);
            }
        }
    }
}

