
using SmartAttendance.Data;
using SmartAttendance.Models;
using SmartAttendance.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SmartAttendance.Services
{
    public class CourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        public List<CourseDto> GetAll()
        {
            return _context.Courses
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .AsNoTracking()
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    InstructorName = c.Instructor.User.Name
                })
                .ToList();
        }

        public CourseDetailDto? GetById(int id)
        {
            var course = _context.Courses
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return null;

            var studentCount = _context.Enrollments
                .Count(e => e.CourseId == id);

            return new CourseDetailDto
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor.User.Name,
                EnrolledStudentsCount = studentCount
            };
        }

        public CourseDto? Create(CreateCourseDto dto)
        {
            var instructor = _context.Instructors
                .Include(i => i.User)
                .FirstOrDefault(i => i.Id == dto.InstructorId);

            if (instructor == null) return null;

            var course = new Course
            {
                Code = dto.Code,
                Name = dto.Name,
                InstructorId = instructor.Id
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            return new CourseDto
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                InstructorName = instructor.User.Name
            };
        }

        public bool Update(int id, UpdateCourseDto dto)
        {
            var instructor = _context.Instructors
                .FirstOrDefault(i => i.Id == dto.InstructorId);
            if (instructor == null) return false;

            var course = _context.Courses.Find(id);
            if (course == null) return false;

            course.Code = dto.Code;
            course.Name = dto.Name;
            course.InstructorId = instructor.Id;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return false;

            _context.Courses.Remove(course);
            _context.SaveChanges();
            return true;
        }
    }
}
