using Microsoft.AspNetCore.Mvc;
using SmartAttendance.Services;
using SmartAttendance.DTOs;
using Microsoft.AspNetCore.Authorization;

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

        [HttpPost]
        public IActionResult Create([FromBody] CreateClassroomDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            var created = _service.Create(dto);
            if (created == null) return BadRequest("Unable to create classroom");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateClassroomDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            if (!_service.Update(id, dto)) return NotFound("Classroom not found");
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_service.Delete(id)) return NotFound("Classroom not found");
            return NoContent();
        }
    }
}
