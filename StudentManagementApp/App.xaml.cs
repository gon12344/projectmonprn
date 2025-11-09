using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Factories;
using WpfApp1.ViewModels;
using WpfApp1.Services;

namespace WpfApp1
{
    public partial class App : Application
    {
        public static DBContext? DbContext { get; private set; }
        public static ICourseFactory CourseFactory { get; private set; } = new CourseFactory();
        public static WpfApp1.Models.User? CurrentUser { get; set; }
        
        public ServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var sql = "Server=.;Database=QLSV_PRN;User Id=sa;Password=123456;TrustServerCertificate=True";
            var services = new ServiceCollection();
 
            // Configure DbContext
            services.AddDbContext<DBContext>(options =>
                 options.UseSqlServer(sql));
        
            // Register Services
            services.AddSingleton<StudentCollectionService>();
 
            // Register ViewModels
            services.AddTransient<StudentManagementViewModel>();
            services.AddTransient<CourseManagementViewModel>();
            services.AddTransient<EnrollmentViewModel>();
            
            ServiceProvider = services.BuildServiceProvider();

            // Get DbContext instance for backward compatibility
            DbContext = ServiceProvider.GetRequiredService<DBContext>();
            
            // Ensure database is created
            DbContext.Database.EnsureCreated();
        }
 
        protected override void OnExit(ExitEventArgs e)
        {
            if (ServiceProvider is IDisposable disposable)
                disposable.Dispose();
            base.OnExit(e);
        }
    }
}
