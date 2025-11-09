using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupViewModels();
        }

        private void SetupViewModels()
        {
            var serviceProvider = ((App)Application.Current).ServiceProvider;
            
            // Set DataContext for each view
            var studentManagementView = FindViewInTab<StudentManagementView>(0);
            if (studentManagementView != null)
                studentManagementView.DataContext = serviceProvider.GetRequiredService<StudentManagementViewModel>();

            var courseManagementView = FindViewInTab<CourseManagementView>(1);
            if (courseManagementView != null)
                courseManagementView.DataContext = serviceProvider.GetRequiredService<CourseManagementViewModel>();

            var enrollmentView = FindViewInTab<EnrollmentView>(2);
            if (enrollmentView != null)
            {
                var viewModel = serviceProvider.GetRequiredService<EnrollmentViewModel>();
                enrollmentView.DataContext = viewModel;
                // Initialize enrollment view async
                _ = Task.Run(async () =>
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        await viewModel.InitializeAsync();
                    });
                });
            }
        }

        private T? FindViewInTab<T>(int tabIndex) where T : class
        {
            var tabControl = Content as TabControl;
            if (tabControl?.Items[tabIndex] is TabItem tabItem)
            {
                return tabItem.Content as T;
            }
            return null;
        }
    }
}