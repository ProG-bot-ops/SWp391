using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicineController : ControllerBase
{
    private readonly IMedicineService _service;

    public MedicineController(IMedicineService service)
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
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("add-medicine")]
    public async Task<IActionResult> Create([FromBody] MedicineRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("update-medicine/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MedicineRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("delete-medicine/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("prescribed/{value}")]
    public async Task<IActionResult> GetByPrescribed(int value)
    {
        var result = await _service.GetByPrescribedAsync(value);    
        return Ok(result);
    }

    [HttpGet("get-by-category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _service.GetByCategoryIdAsync(categoryId);
        return Ok(result);
    }


}
