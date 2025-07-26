using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.Services;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShiftRequestController : ControllerBase
{
    private readonly IShiftRequestService _service;

    public ShiftRequestController(IShiftRequestService service)
    {
        _service = service;
    }

    // Gửi yêu cầu đổi ca/nghỉ ca
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] ShiftRequestRequestDTO request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Lấy danh sách yêu cầu của bác sĩ hiện tại
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetByDoctorAsync(int doctorId)
    {
        var result = await _service.GetByDoctorAsync(doctorId);
        return Ok(result);
    }

    // Lấy tất cả yêu cầu (cho quản lý)
    [HttpGet("all")]
   // [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // Duyệt yêu cầu
    [HttpPut("{requestId}/approve")]
    public async Task<IActionResult> ApproveAsync(int requestId)
    {
        try
        {
            var success = await _service.ApproveAsync(requestId);
            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Từ chối yêu cầu
    [HttpPut("{requestId}/reject")]
    public async Task<IActionResult> RejectAsync(int requestId, [FromBody] string? reason)
    {
        try
        {
            var success = await _service.RejectAsync(requestId, reason);
            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}