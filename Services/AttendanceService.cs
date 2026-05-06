
using SmartAttendance.Data;
using SmartAttendance.Models;
using SmartAttendance.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SmartAttendance.Services
{
    public class AttendanceService
    {
        private readonly AppDbContext _context;

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public List<AttendanceDto> GetAll()
        {
            return _context.Attendances
                .Include(a => a.Course)
                .AsNoTracking()
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

        public AttendanceDto? GetById(int id)
        {
            var attendance = _context.Attendances
                .Include(a => a.Course)
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == id);

            if (attendance == null) return null;

            var student = _context.Students
                .Include(s => s.User)
                .FirstOrDefault(s => s.Id == attendance.StudentId);

            return new AttendanceDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                StudentName = student.User.Name,
                CourseId = attendance.CourseId,
                CourseName = attendance.Course.Name,
                IsPresent = attendance.IsPresent,
                Date = attendance.Date
            };
        }

        public int? GetStudentIdByUserId(string userId)
        {
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);
            return student?.Id;
        }

        public List<AttendanceDto> GetByStudentId(int studentId)
        {
            return _context.Attendances
                .Include(a => a.Course)
                .AsNoTracking()
                .Where(a => a.StudentId == studentId)
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

        public List<AttendanceDto> GetByCourseId(int courseId)
        {
            return _context.Attendances
                .Include(a => a.Course)
                .AsNoTracking()
                .Where(a => a.CourseId == courseId)
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

        public AttendanceDto? Create(CreateAttendanceDto dto)
        {
            var student = _context.Students
                .Include(s => s.User)
                .FirstOrDefault(s => s.Id == dto.StudentId);

            var course = _context.Courses.Find(dto.CourseId);

            if (student == null || course == null) return null;

            var attendance = new Attendance
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                IsPresent = dto.IsPresent,
                Date = dto.Date
            };

            _context.Attendances.Add(attendance);
            _context.SaveChanges();

            return new AttendanceDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                StudentName = student.User.Name,
                CourseId = attendance.CourseId,
                CourseName = course.Name,
                IsPresent = attendance.IsPresent,
                Date = attendance.Date
            };
        }

        public bool Update(int id, UpdateAttendanceDto dto)
        {
            var attendance = _context.Attendances.Find(id);
            if (attendance == null) return false;

            attendance.IsPresent = dto.IsPresent;
            attendance.Date = dto.Date;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var attendance = _context.Attendances.Find(id);
            if (attendance == null) return false;

            _context.Attendances.Remove(attendance);
            _context.SaveChanges();
            return true;
        }
    }
}
