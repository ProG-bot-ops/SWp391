using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.DoctorShift;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorScheduleController : ControllerBase
{
    private readonly IDoctorScheduleService _service;

    public DoctorScheduleController(IDoctorScheduleService service)
    {
        _service = service;
    }

    // Tạo mới ca trực
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] DoctorScheduleCreate request)
    {
        var result = await _service.CreateAsync(request);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    // Cập nhật ca trực
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] DoctorScheduleUpdate request)
    {
        var result = await _service.UpdateAsync(request);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    // Xóa ca trực
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] DoctorScheduleDelete request)
    {
        var result = await _service.DeleteAsync(request);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
}