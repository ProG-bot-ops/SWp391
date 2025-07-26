using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Department;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentController(IDepartmentService service)
    {
        _service = service;
    }

    [HttpPost("add-department")]
    [ProducesResponseType(typeof(IEnumerable<Department>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> AddDepartment([FromBody] DepartmentCreate create)
    {
        try
        {
            var response = await _service.CreateDepartmentAsync(create);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("get-all")]
    [ProducesResponseType(typeof(IEnumerable<Department>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartment()
    {
        try
        {
            var response = await _service.GetAllDepartmentAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-by-name/{name}")]
    [ProducesResponseType(typeof(IEnumerable<Department>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindByName(string name)
    {
        try
        {
            var response = await _service.SearchDepartmentByKeyAsync(name);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-id/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Department>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindById(int id)
    {
        try
        {
            var response = await _service.FindDepartmentByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Department>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[AllowAnonymous]
    public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentUpdate update, int id)
    {
        try
        {
            var response = await _service.UpdateDepartmentAsync(id, update);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Department>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[Authorize(Roles = "Admin, Employee")]
    public async Task<IActionResult> SoftDeleteDepartment(int id, DepartmentStatus newStatus)
    {
        try
        {
            var response = await _service.SoftDeleteDepartmentAsync(id, newStatus);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpDelete("delete-permanent/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Department>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[AllowAnonymous]
    public async Task<IActionResult> HardDeleteDepartment(int id)
    {
        try
        {
            var response = await _service.HardDeleteDepartmentAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }
}
