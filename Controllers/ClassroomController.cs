using Microsoft.AspNetCore.Mvc;
using SmartAttendance.Services;
using SmartAttendance.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SmartAttendance.Controllers
{
    [ApiController]
    [Route("api/classrooms")]
    public class ClassroomController : ControllerBase
    {
        private readonly ClassroomService _service;

        public ClassroomController(ClassroomService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var classroom = _service.GetById(id);
            if (classroom == null) return NotFound("Classroom not found");
            return Ok(classroom);
        }

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateClassroomDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            var created = _service.Create(dto);
            if (created == null) return BadRequest("Unable to create classroom");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateClassroomDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            if (!_service.Update(id, dto)) return NotFound("Classroom not found");
            return NoContent();
        }

        [Authorize(Roles = "Instructor", AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_service.Delete(id)) return NotFound("Classroom not found");
            return NoContent();
        }
    }
}
