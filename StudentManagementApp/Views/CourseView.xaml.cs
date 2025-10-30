using System.Windows.Controls;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class CourseView : UserControl
    {
        public CourseView()
        {
            InitializeComponent();
            if (App.DbContext != null)
            {
                DataContext = new CourseViewModel(App.DbContext, App.CourseFactory);
            }
        }
    }
}
