using WpfApp1.Models;

namespace WpfApp1.Factories
{
    public interface ICourseFactory
    {
        Course CreateCourse(string courseCode, string courseName, int credits, string? department = null, string? description = null);
        Course CreateCourseWithDefaults(string courseCode, string courseName);
    }

    public class CourseFactory : ICourseFactory
    {
        public Course CreateCourse(string courseCode, string courseName, int credits, string? department = null, string? description = null)
        {
            return new Course
            {
                CourseCode = courseCode,
                CourseName = courseName,
                Credits = credits,
                Department = department ?? "General",
                Description = description,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
        }

        public Course CreateCourseWithDefaults(string courseCode, string courseName)
        {
            return CreateCourse(courseCode, courseName, 3, "General", "Course description not provided");
        }
    }

    // Factory cho các loại môn học khác nhau
    public class SpecializedCourseFactory : ICourseFactory
    {
        private readonly string _department;
        private readonly int _defaultCredits;

        public SpecializedCourseFactory(string department, int defaultCredits = 3)
        {
            _department = department;
            _defaultCredits = defaultCredits;
        }

        public Course CreateCourse(string courseCode, string courseName, int credits, string? department = null, string? description = null)
        {
            return new Course
            {
                CourseCode = courseCode,
                CourseName = courseName,
                Credits = credits,
                Department = department ?? _department,
                Description = description,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
        }

        public Course CreateCourseWithDefaults(string courseCode, string courseName)
        {
            return CreateCourse(courseCode, courseName, _defaultCredits, _department, $"{_department} specialized course");
        }
    }
}
