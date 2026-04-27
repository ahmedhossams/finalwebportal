using SmartAttendance.Data;
using SmartAttendance.Models;
using SmartAttendance.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SmartAttendance.Services
{
    public class InstructorService
    {
        private readonly AppDbContext _context;

        public InstructorService(AppDbContext context)
        {
            _context = context;
        }

        public List<InstructorDto> GetAll()
        {
            return _context.Instructors
                .Include(i => i.User)
                .AsNoTracking()
                .Select(i => new InstructorDto
                {
                    Id = i.Id,
                    Name = i.User.Name,
                    Email = i.User.Email
                })
                .ToList();
        }

        public InstructorDto? GetById(int id)
        {
            return _context.Instructors
                .Include(i => i.User)
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new InstructorDto
                {
                    Id = i.Id,
                    Name = i.User.Name,
                    Email = i.User.Email
                })
                .FirstOrDefault();
        }

        /* ===================================================================
        OBSOLETE METHOD: 
        User creation is now handled securely via ASP.NET Core Identity 
        inside the AuthController. Do not create users manually here!
        ===================================================================
        public InstructorDto? Create(CreateInstructorDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Instructor"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var instructor = new Instructor { UserId = user.Id };
            _context.Instructors.Add(instructor);
            _context.SaveChanges();

            return new InstructorDto
            {
                Id = instructor.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
        */

        public bool Delete(int id)
        {
            var instructor = _context.Instructors.Include(i => i.User).FirstOrDefault(i => i.Id == id);
            if (instructor == null) return false;

            _context.Instructors.Remove(instructor);
            if (instructor.User != null)
            {
                _context.Users.Remove(instructor.User);
            }
            _context.SaveChanges();
            return true;
        }
    }
}