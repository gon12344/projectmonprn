using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Factories;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DBContext? DbContext { get; private set; }
        public static ICourseFactory CourseFactory { get; private set; } = new CourseFactory();
        public static WpfApp1.Models.User? CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure SQL Server 
            var sql = "Server=localhost;Database=QLSV_PRN;User Id=sa;Password=123;TrustServerCertificate=True";
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseSqlServer(sql)
                .Options;

            DbContext = new DBContext(options);

            // Ensure database and seed initial data if needed
            DbContext.Database.EnsureCreated();

            // Additional bootstrap seeding if required (HasData requires migrations; EnsureCreated will create with current model)
        }
    }

}
