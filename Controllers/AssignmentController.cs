using Microsoft.AspNetCore.Mvc;
using SmartAttendance.Services;
using SmartAttendance.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SmartAttendance.Controllers
{
    [ApiController]
    [Route("api/assignments")]
    public class AssignmentController : ControllerBase
    {
        private readonly AssignmentService _service;

        public AssignmentController(AssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var assignment = _service.GetById(id);
            if (assignment == null) return NotFound("Assignment not found");
            return Ok(assignment);
        }

        [HttpGet("course/{courseId}")]
        public IActionResult GetByCourseId(int courseId) => Ok(_service.GetByCourseId(courseId));

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateAssignmentDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            var created = _service.Create(dto);
            if (created == null) return BadRequest("Unable to create assignment");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAssignmentDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            if (!_service.Update(id, dto)) return NotFound("Assignment not found");
            return NoContent();
        }

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_service.Delete(id)) return NotFound("Assignment not found");
            return NoContent();
        }
    }
}
