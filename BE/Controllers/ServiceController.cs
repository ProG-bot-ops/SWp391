using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Service;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        try
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceById(int id)
    {
        try
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
                return NotFound(new { message = "Service not found" });

            return Ok(service);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("department/{departmentId}")]
    public async Task<IActionResult> GetServicesByDepartment(int departmentId)
    {
        try
        {
            var services = await _serviceService.GetServicesByDepartmentAsync(departmentId);
            return Ok(services);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] ServiceRequestDTO request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = await _serviceService.CreateServiceAsync(request);
            return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceRequestDTO request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = await _serviceService.UpdateServiceAsync(id, request);
            if (service == null)
                return NotFound(new { message = "Service not found" });

            return Ok(service);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        try
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            if (!result)
                return NotFound(new { message = "Service not found" });

            return Ok(new { message = "Service deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
} 