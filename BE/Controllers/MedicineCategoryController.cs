using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Service;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicineCategoryController : ControllerBase
{
    private readonly IMedicineCategoryService _service;

    public MedicineCategoryController(IMedicineCategoryService service)
    {
        _service = service;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost("add-medicineCategory")]
    public async Task<IActionResult> Create([FromBody] MedicineCategoryRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("update-medicineCategory/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MedicineCategoryRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("delete-medicineCategory/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound();
        return NoContent();
    }
}
