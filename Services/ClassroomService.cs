using SmartAttendance.Data;
using SmartAttendance.Models;
using SmartAttendance.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SmartAttendance.Services
{
    public class ClassroomService
    {
        private readonly AppDbContext _context;

        public ClassroomService(AppDbContext context)
        {
            _context = context;
        }

        public List<ClassroomDto> GetAll()
        {
            return _context.Classrooms
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .AsNoTracking()
                .Select(c => new ClassroomDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Capacity = c.Capacity,
                    InstructorName = c.Instructor.User.Name
                })
                .ToList();
        }

        public ClassroomDto? GetById(int id)
        {
            return _context.Classrooms
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ClassroomDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Capacity = c.Capacity,
                    InstructorName = c.Instructor.User.Name
                })
                .FirstOrDefault();
        }

        public ClassroomDto? Create(CreateClassroomDto dto)
        {
            var instructor = _context.Instructors
                .Include(i => i.User)
                .FirstOrDefault(i => i.Id == dto.InstructorId);

            if (instructor == null) return null;

            var classroom = new Classroom
            {
                Name = dto.Name,
                Location = dto.Location,
                Capacity = dto.Capacity,
                InstructorId = instructor.Id
            };

            _context.Classrooms.Add(classroom);
            _context.SaveChanges();

            return new ClassroomDto
            {
                Id = classroom.Id,
                Name = classroom.Name,
                Location = classroom.Location,
                Capacity = classroom.Capacity,
                InstructorName = instructor.User.Name
            };
        }

        public bool Update(int id, UpdateClassroomDto dto)
        {
            var instructor = _context.Instructors
                .FirstOrDefault(i => i.Id == dto.InstructorId);
            if (instructor == null) return false;

            var classroom = _context.Classrooms.Find(id);
            if (classroom == null) return false;

            classroom.Name = dto.Name;
            classroom.Location = dto.Location;
            classroom.Capacity = dto.Capacity;
            classroom.InstructorId = instructor.Id;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var classroom = _context.Classrooms.Find(id);
            if (classroom == null) return false;

            _context.Classrooms.Remove(classroom);
            _context.SaveChanges();
            return true;
        }
    }
}
