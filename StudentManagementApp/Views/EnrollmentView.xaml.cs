using System.Windows.Controls;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class EnrollmentView : UserControl
    {
        private EnrollmentViewModel? _vm;
        public EnrollmentView()
        {
            InitializeComponent();
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
    }
}
