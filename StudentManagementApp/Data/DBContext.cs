using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Student entity - map to existing SQL columns in QLSV_PRN
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.StudentCode).IsRequired().HasMaxLength(255).HasColumnName("StudentCode");
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(255).HasColumnName("FullName");
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("Gender");
                entity.Property(e => e.Address).HasMaxLength(255).HasColumnName("StudentAddress");
                entity.Property(e => e.DateOfBirth).HasColumnName("DOB");

                // Optional columns may not exist in your current SQL schema
                entity.Ignore(e => e.Email);
                entity.Ignore(e => e.PhoneNumber);
            });

            // Configure Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CourseName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Department).HasMaxLength(50);
            });

            // Configure Enrollment entity
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(200);

                // *** NEW MAPPING FOR GRADE ***
                entity.Property(e => e.Grade).HasColumnType("decimal(3, 1)");
                entity.Property(e => e.GradedDate);
                // *** END NEW MAPPING ***

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            // Configure User entity mapping to existing table 'Users'
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.HasOne(u => u.Role)
                    .WithMany()
                    .HasForeignKey(u => u.RoleId);
                entity.HasOne(u => u.Student)
                    .WithMany()
                    .HasForeignKey(u => u.StudentId)
                    .IsRequired(false);
            });

            // No code-first seeding when using existing SQL schema
        }

    }
}
