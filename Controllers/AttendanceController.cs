using Microsoft.AspNetCore.Mvc;
using SmartAttendance.Services;
using SmartAttendance.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartAttendance.Models;
using System.Security.Claims;

namespace SmartAttendance.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _service;
        private readonly UserManager<User> _userManager;

        public AttendanceController(AttendanceService service, UserManager<User> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Safely get the user from the current claims principal
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new { message = "User session not found." });

            var roles = await _userManager.GetRolesAsync(user);
            
            // Check if user is a student (case-insensitive)
            bool isStudent = roles.Any(r => r.Equals("Student", StringComparison.OrdinalIgnoreCase));

            if (isStudent)
            {
                var studentId = _service.GetStudentIdByUserId(user.Id);
                if (studentId == null) return Ok(new List<AttendanceDto>());
                
                // Returns only this student's records
                return Ok(_service.GetByStudentId(studentId.Value));
            }

            // If Instructor/Admin, return all records
            return Ok(_service.GetAll());
        }

        [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var attendance = _service.GetById(id);
            if (attendance == null) return NotFound("Attendance record not found");
            return Ok(attendance);
        }

        [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpGet("course/{courseId}")]
        public IActionResult GetByCourseId(int courseId) => Ok(_service.GetByCourseId(courseId));

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateAttendanceDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            var created = _service.Create(dto);
            if (created == null) return BadRequest("Unable to create attendance record");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAttendanceDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            if (!_service.Update(id, dto)) return NotFound("Attendance record not found");
            return NoContent();
        }

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_service.Delete(id)) return NotFound("Attendance record not found");
            return NoContent();
        }
    }
}