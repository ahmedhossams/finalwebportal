using Microsoft.AspNetCore.Mvc;
using SmartAttendance.Services;
using SmartAttendance.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace SmartAttendance.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _service;

        public AttendanceController(AttendanceService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var attendance = _service.GetById(id);
            if (attendance == null) return NotFound("Attendance record not found");
            return Ok(attendance);
        }

        [HttpGet("course/{courseId}")]
        public IActionResult GetByCourseId(int courseId) => Ok(_service.GetByCourseId(courseId));

        [HttpPost]
        public IActionResult Create([FromBody] CreateAttendanceDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            var created = _service.Create(dto);
            if (created == null) return BadRequest("Unable to create attendance record");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAttendanceDto dto)
        {
            if (dto == null) return BadRequest("Invalid request");
            if (!_service.Update(id, dto)) return NotFound("Attendance record not found");
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_service.Delete(id)) return NotFound("Attendance record not found");
            return NoContent();
        }
    }
}
