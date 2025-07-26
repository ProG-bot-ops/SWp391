using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorShiftFillerController : ControllerBase
{
    private readonly IDoctorShiftFillerService _service;

    public DoctorShiftFillerController(IDoctorShiftFillerService service)
    {
        _service = service;
    }

    // Lấy danh sách ca trực theo filter (ngày/tuần/tháng)
    [HttpPost("list")]
    public async Task<IActionResult> GetDoctorShifts([FromBody] DoctorShiftFilterRequestDTO filter)
    {
        try
        {
            var result = await _service.GetDoctorShiftsAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Lấy chi tiết ca trực theo Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy ca trực." });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}