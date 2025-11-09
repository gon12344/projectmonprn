using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    /// <summary>
    /// Service s? d?ng Collection Generics ?? qu?n lý danh sách sinh viên và l?p h?c
    /// </summary>
    public class StudentCollectionService
    {
        // S? d?ng List<T> - Collection Generic c? b?n
        private readonly List<Student> _students;
        private readonly List<Course> _courses;
        private readonly List<Enrollment> _enrollments;

        // S? d?ng Dictionary<TKey, TValue> ?? t?i ?u tìm ki?m
        private readonly Dictionary<int, Student> _studentLookup;
        private readonly Dictionary<int, Course> _courseLookup;

        // S? d?ng ObservableCollection<T> cho WPF Data Binding
        public ObservableCollection<Student> Students { get; }
      public ObservableCollection<Course> Courses { get; }
        public ObservableCollection<Enrollment> Enrollments { get; }

        public StudentCollectionService()
 {
   _students = new List<Student>();
     _courses = new List<Course>();
 _enrollments = new List<Enrollment>();
      
       _studentLookup = new Dictionary<int, Student>();
      _courseLookup = new Dictionary<int, Course>();

            Students = new ObservableCollection<Student>();
            Courses = new ObservableCollection<Course>();
            Enrollments = new ObservableCollection<Enrollment>();
        }

        #region Student Management using List<T>
        
        public void AddStudent(Student student)
{
          if (student == null) throw new ArgumentNullException(nameof(student));
       
            _students.Add(student);
            Students.Add(student);
 
       if (student.StudentId > 0)
          _studentLookup[student.StudentId] = student;
        }

      public bool RemoveStudent(Student student)
     {
       if (student == null) return false;
            
      var removed = _students.Remove(student);
            if (removed)
  {
          Students.Remove(student);
     _studentLookup.Remove(student.StudentId);
          }
  return removed;
        }

        public Student? FindStudentById(int studentId)
    {
            // S? d?ng Dictionary ?? tìm ki?m nhanh O(1)
            return _studentLookup.TryGetValue(studentId, out var student) ? student : null;
        }

  public IEnumerable<Student> FindStudentsByName(string name)
        {
   // S? d?ng LINQ v?i Collection Generic
    return _students.Where(s => s.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

    public IEnumerable<Student> GetStudentsOrderedByCode()
        {
      // S? d?ng LINQ OrderBy v?i Collection
          return _students.OrderBy(s => s.StudentCode);
        }

        #endregion

      #region Course Management using List<T>

  public void AddCourse(Course course)
    {
            if (course == null) throw new ArgumentNullException(nameof(course));
         
          _courses.Add(course);
 Courses.Add(course);
     
 if (course.CourseId > 0)
           _courseLookup[course.CourseId] = course;
        }

        public bool RemoveCourse(Course course)
      {
            if (course == null) return false;
            
        var removed = _courses.Remove(course);
            if (removed)
 {
         Courses.Remove(course);
      _courseLookup.Remove(course.CourseId);
     }
            return removed;
        }

        public Course? FindCourseById(int courseId)
        {
            return _courseLookup.TryGetValue(courseId, out var course) ? course : null;
        }

        public IEnumerable<Course> FindCoursesByName(string name)
   {
    return _courses.Where(c => c.CourseName.Contains(name, StringComparison.OrdinalIgnoreCase));
      }

        public IEnumerable<Course> GetActiveCoursesOrderedByCode()
        {
         return _courses.Where(c => c.IsActive).OrderBy(c => c.CourseCode);
}

   #endregion

        #region Enrollment Management using List<T>

        public void AddEnrollment(Enrollment enrollment)
        {
       if (enrollment == null) throw new ArgumentNullException(nameof(enrollment));
            
   _enrollments.Add(enrollment);
          Enrollments.Add(enrollment);
        }

        public bool RemoveEnrollment(Enrollment enrollment)
        {
     if (enrollment == null) return false;
     
         var removed = _enrollments.Remove(enrollment);
 if (removed)
                Enrollments.Remove(enrollment);
    return removed;
        }

        public IEnumerable<Enrollment> GetEnrollmentsByStudentId(int studentId)
        {
        return _enrollments.Where(e => e.StudentId == studentId);
        }

        public IEnumerable<Enrollment> GetEnrollmentsByCourseId(int courseId)
        {
            return _enrollments.Where(e => e.CourseId == courseId);
        }

        public IEnumerable<Student> GetStudentsInCourse(int courseId)
        {
      // S? d?ng Join v?i Collection Generics
          return from enrollment in _enrollments
        join student in _students on enrollment.StudentId equals student.StudentId
           where enrollment.CourseId == courseId
       select student;
  }

 #endregion

        #region Statistical Methods using Collection Generics

   public int GetTotalStudents() => _students.Count;
        
     public int GetTotalCourses() => _courses.Count;
        
        public int GetActiveCoursesCount() => _courses.Count(c => c.IsActive);
        
   public double GetAverageStudentsPerCourse()
     {
     if (_courses.Count == 0) return 0;
          
         var enrollmentGroups = _enrollments.GroupBy(e => e.CourseId);
            return enrollmentGroups.Average(g => g.Count());
        }

        public IEnumerable<(Course Course, int StudentCount)> GetCoursesWithStudentCount()
        {
            // S? d?ng GroupBy và Join v?i Collection Generics
            var enrollmentCounts = _enrollments
     .GroupBy(e => e.CourseId)
       .ToDictionary(g => g.Key, g => g.Count());

return _courses.Select(course => (
  Course: course,
     StudentCount: enrollmentCounts.GetValueOrDefault(course.CourseId, 0)
  ));
        }

        public IEnumerable<Student> GetStudentsWithoutEnrollments()
        {
            // S? d?ng Except v?i Collection Generics
         var enrolledStudentIds = _enrollments.Select(e => e.StudentId).Distinct();
            return _students.Where(s => !enrolledStudentIds.Contains(s.StudentId));
        }

        #endregion

   #region Bulk Operations using Collection Generics

        public void LoadStudentsFromCollection(IEnumerable<Student> students)
        {
     _students.Clear();
      Students.Clear();
            _studentLookup.Clear();

            foreach (var student in students)
  {
   AddStudent(student);
    }
        }

        public void LoadCoursesFromCollection(IEnumerable<Course> courses)
        {
          _courses.Clear();
     Courses.Clear();
       _courseLookup.Clear();

          foreach (var course in courses)
       {
   AddCourse(course);
      }
      }

   public void LoadEnrollmentsFromCollection(IEnumerable<Enrollment> enrollments)
        {
            _enrollments.Clear();
     Enrollments.Clear();

  foreach (var enrollment in enrollments)
            {
    AddEnrollment(enrollment);
      }
        }

        #endregion

    #region Search and Filter using Collection Generics

        public IEnumerable<Student> SearchStudents(string searchTerm)
        {
       if (string.IsNullOrWhiteSpace(searchTerm))
     return _students;

      return _students.Where(s =>
           s.StudentCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
     s.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
          (s.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
}

     public IEnumerable<Course> SearchCourses(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
    return _courses;

 return _courses.Where(c =>
      c.CourseCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
      c.CourseName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        (c.Department?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        #endregion
    }
}