using SmartAttendance.Data;
using SmartAttendance.Models;
using SmartAttendance.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SmartAttendance.Services
{
    public class AssignmentService
    {
        private readonly AppDbContext _context;

        public AssignmentService(AppDbContext context)
        {
            _context = context;
        }

        public List<AssignmentDto> GetAll()
        {
            return _context.Assignments
                .Include(a => a.Course)
                .AsNoTracking()
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate,
                    CourseName = a.Course.Name,
                    CourseId = a.CourseId
                })
                .ToList();
        }

        public List<AssignmentDto> GetByCourseId(int courseId)
        {
            return _context.Assignments
                .Include(a => a.Course)
                .AsNoTracking()
                .Where(a => a.CourseId == courseId)
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate,
                    CourseName = a.Course.Name,
                    CourseId = a.CourseId
                })
                .ToList();
        }

         public AssignmentDto? GetById(int id)
        {
            return _context.Assignments
                .Include(a => a.Course)
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate,
                    CourseName = a.Course.Name,
                    CourseId = a.CourseId
                })
                .FirstOrDefault();
        }

        public AssignmentDto? Create(CreateAssignmentDto dto)
        {
            var course = _context.Courses.Find(dto.CourseId);
            if (course == null) return null;

            var assignment = new Assignment
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate
            };

            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            return new AssignmentDto
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                CourseName = course.Name
            };
        }

        public bool Update(int id, UpdateAssignmentDto dto)
        {
            var assignment = _context.Assignments.Find(id);
            if (assignment == null) return false;

            assignment.Title = dto.Title;
            assignment.Description = dto.Description;
            assignment.DueDate = dto.DueDate;
            assignment.CourseId = dto.CourseId;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var assignment = _context.Assignments.Find(id);
            if (assignment == null) return false;

            _context.Assignments.Remove(assignment);
            _context.SaveChanges();
            return true;
        }
    }
}
