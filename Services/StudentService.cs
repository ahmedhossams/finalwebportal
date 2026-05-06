
using SmartAttendance.Data;
using SmartAttendance.Models;
using SmartAttendance.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SmartAttendance.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public List<StudentDto> GetAll()
        {
            return _context.Students
                .Include(s => s.User)
                .AsNoTracking()
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.User.Name,
                    Email = s.User.Email
                })
                .ToList();
        }

        public StudentDetailDto? GetById(int id)
        {
            var student = _context.Students
                .Include(s => s.User)
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == id);

            if (student == null) return null;

            var enrollments = _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Instructor)
                .ThenInclude(i => i.User)
                .Where(e => e.StudentId == id)
                .Select(e => new StudentCourseDto
                {
                    CourseId = e.Course.Id,
                    CourseName = e.Course.Name,
                    InstructorName = e.Course.Instructor.User.Name
                })
                .ToList();

            return new StudentDetailDto
            {
                Id = student.Id,
                Name = student.User.Name,
                Email = student.User.Email,
                EnrolledCourses = enrollments
            };
        }

        public StudentDetailDto? GetByUserId(string userId)
        {
            var student = _context.Students
                .Include(s => s.User)
                .AsNoTracking()
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null) return null;

            var enrollments = _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Instructor)
                .ThenInclude(i => i.User)
                .Where(e => e.StudentId == student.Id)
                .Select(e => new StudentCourseDto
                {
                    CourseId = e.Course.Id,
                    CourseName = e.Course.Name,
                    InstructorName = e.Course.Instructor.User.Name
                })
                .ToList();

            return new StudentDetailDto
            {
                Id = student.Id,
                Name = student.User.Name,
                Email = student.User.Email,
                EnrolledCourses = enrollments
            };
        }

        public List<CourseDto> GetStudentCoursesByUserId(string userId)
        {
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);
            if (student == null) return new List<CourseDto>();

            return _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Instructor)
                .ThenInclude(i => i.User)
                .AsNoTracking()
                .Where(e => e.StudentId == student.Id)
                .Select(e => new CourseDto
                {
                    Id = e.Course.Id,
                    Name = e.Course.Name,
                    InstructorName = e.Course.Instructor.User.Name
                })
                .ToList();
        }

        public List<AttendanceDto> GetStudentAttendanceByUserId(string userId)
        {
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);
            if (student == null) return new List<AttendanceDto>();

            return _context.Attendances
                .Include(a => a.Course)
                .AsNoTracking()
                .Where(a => a.StudentId == student.Id)
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentName = _context.Students.Include(s => s.User).FirstOrDefault(s => s.Id == a.StudentId).User.Name,
                    CourseId = a.CourseId,
                    CourseName = a.Course.Name,
                    IsPresent = a.IsPresent,
                    Date = a.Date
                })
                .ToList();
        }

        public List<AssignmentDto> GetStudentAssignmentsByUserId(string userId)
        {
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);
            if (student == null) return new List<AssignmentDto>();

            var courseIds = _context.Enrollments
                .Where(e => e.StudentId == student.Id)
                .Select(e => e.CourseId)
                .ToList();

            return _context.Assignments
                .Include(a => a.Course)
                .AsNoTracking()
                .Where(a => courseIds.Contains(a.CourseId))
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate,
                    CourseName = a.Course.Name
                })
                .ToList();
        }

        public bool EnrollInCourse(int studentId, int courseId)
        {
            var student = _context.Students.Find(studentId);
            var course = _context.Courses.Find(courseId);

            if (student == null || course == null) return false;

            var existingEnrollment = _context.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment != null) return false;

            var enrollment = new Enrollment { StudentId = studentId, CourseId = courseId };
            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
            return true;
        }

        public List<EnrollmentDto>? GetStudentEnrollments(int studentId)
        {
            var student = _context.Students.Find(studentId);
            if (student == null) return null;

            return _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Course)
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentDto
                {
                    StudentId = e.StudentId,
                    StudentName = _context.Students.Include(s => s.User).FirstOrDefault(s => s.Id == e.StudentId).User.Name,
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name
                })
                .ToList();
        }
    }
}
