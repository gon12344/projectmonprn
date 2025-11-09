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

            // Student entity - mapping to existing database table
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.StudentCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200).HasColumnName("StudentAddress");
                entity.Property(e => e.DateOfBirth).IsRequired().HasColumnName("DOB");
            });

            // Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CourseName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Department).HasMaxLength(50);
                entity.Property(e => e.Credits).HasDefaultValue(3);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
            });

            // Enrollment entity
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollments");
                entity.HasKey(e => e.EnrollmentId);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.Notes).HasMaxLength(200);
                entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
            });

            // User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(u => u.Role)
                    .WithMany()
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Student)
                    .WithMany()
                    .HasForeignKey(u => u.StudentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }
    }
}
