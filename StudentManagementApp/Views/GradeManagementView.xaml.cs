using System.Windows.Controls;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class GradeManagementView : UserControl
    {
        private GradeManagementViewModel? _vm; // Giữ tham chiếu đến VM

        public GradeManagementView()
        {
            InitializeComponent();
            
            if (App.DbContext != null && App.GradeUpdateService != null)
            {
                _vm = new GradeManagementViewModel(App.DbContext, App.GradeUpdateService);
                DataContext = _vm;

                // Đăng ký sự kiện Loaded
                this.Loaded += async (_, __) =>
                {
                    if (_vm != null)
                    {
                        // Gọi hàm tải dữ liệu SAU KHI View đã tải
                        await _vm.InitializeAsync(); 
                    }
                };
            }
        }
    }
}