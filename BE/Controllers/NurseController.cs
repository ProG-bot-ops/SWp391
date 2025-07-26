using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using System.Threading.Tasks;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NurseController : ControllerBase
    {
        private readonly INurseService _nurseService;

        public NurseController(INurseService nurseService)
        {
            _nurseService = nurseService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllNurses()
        {
            var nurses = await _nurseService.GetAllNursesAsync();
            return Ok(nurses);
        }

        [HttpGet("FindById/{id}")]
        public async Task<IActionResult> GetNurseById(int id)
        {
            var nurse = await _nurseService.GetNurseByIdAsync(id);
            if (nurse == null) return NotFound();
            return Ok(nurse);
        }
        [HttpGet("FindByName/{name}")]
        public async Task<IActionResult> GetNurseByName(string name)
        {
            var nurses = await _nurseService.GetNurseByNameAsync(name);
            if (nurses == null || !nurses.Any())
                return NotFound();
            return Ok(nurses);
        }

        [HttpPost("AddNurse")]
        public async Task<IActionResult> CreateNurse([FromBody] NurseCreate nurseCreateDto)
        {
            if (nurseCreateDto == null) return BadRequest();
            var createdNurse = await _nurseService.CreateNurseAsync(nurseCreateDto);
            return CreatedAtAction(nameof(GetNurseById), new { id = createdNurse.Id }, createdNurse);
        }

        [HttpPut("UpdateNurse/{id}")]
        public async Task<IActionResult> UpdateNurse(int id, [FromBody] NurseUpdate nurseUpdateDto)
        {
            if (nurseUpdateDto == null) return BadRequest();
            var updatedNurse = await _nurseService.UpdateNurseAsync(id, nurseUpdateDto);
            if (updatedNurse == null) return NotFound();
            return Ok(updatedNurse);
        }

        [HttpDelete("DeleteNurse/{id}")]
        public async Task<IActionResult> DeleteNurse(int id, [FromBody] NurseDelete nurseDeleteDto)
        {
            if (nurseDeleteDto == null) return BadRequest();
            var result = await _nurseService.DeleteNurseAsync(id, nurseDeleteDto);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}