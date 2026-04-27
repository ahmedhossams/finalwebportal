
using Microsoft.AspNetCore.Mvc;
using SmartAttendance.Services;
using SmartAttendance.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace SmartAttendance.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _service;

        public StudentController(StudentService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _service.GetById(id);
            if (student == null) return NotFound("Student not found");
            return Ok(student);
        }

        [HttpPost("{studentId}/enrollments")]
        public IActionResult CreateEnrollment(int studentId, [FromBody] CreateEnrollmentDto request)
        {
            if (request == null) return BadRequest("Invalid request");
            var result = _service.EnrollInCourse(studentId, request.CourseId);
            if (!result) return BadRequest("Enrollment failed or already exists");
            return Ok(new { message = "Successfully enrolled in course" });
        }

        [Authorize]
        [HttpGet("{studentId}/enrollments")]
        public IActionResult GetEnrollments(int studentId)
        {
            var enrollments = _service.GetStudentEnrollments(studentId);
            if (enrollments == null) return NotFound("Student not found");
            return Ok(enrollments);
        }
    }
}
