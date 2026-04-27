using Microsoft.AspNetCore.Mvc;
using SmartAttendance.Services;
using SmartAttendance.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace SmartAttendance.Controllers
{
    [ApiController]
    [Route("api/instructors")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorService _service;

        public InstructorController(InstructorService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var instructor = _service.GetById(id);
            if (instructor == null) return NotFound("Instructor not found");
            return Ok(instructor);
        }

        /* ===================================================================
        OBSOLETE ENDPOINT: 
        Instructor registration is now handled by the AuthController using 
        ASP.NET Core Identity. Do not use this manual endpoint!
        ===================================================================
        [HttpPost]
        public IActionResult Create([FromBody] CreateInstructorDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            var created = _service.Create(dto);
            if (created == null) return BadRequest("Unable to create instructor");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        */

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_service.Delete(id)) return NotFound("Instructor not found");
            return NoContent();
        }
    }
}
