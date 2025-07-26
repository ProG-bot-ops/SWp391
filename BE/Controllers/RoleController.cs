using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Role;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System.Net;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _service;

    public RoleController(IRoleService service)
    {
        _service = service;
    }

    [HttpPost("add-role")]
    [ProducesResponseType(typeof(IEnumerable<Role>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddRole([FromBody] RoleCreate create)
    {
        try
        {
            var response = await _service.CreateRoleAsync(create);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("get-all")]
    [ProducesResponseType(typeof(IEnumerable<Role>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Role>>> GetAllRole()
    {
        try
        {
            var response = await _service.GetAllRoleAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-by-name/{name}")]
    [ProducesResponseType(typeof(IEnumerable<Role>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindByName(string name)
    {
        try
        {
            var response = await _service.SearchRoleByKeyAsync(name);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-id/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Role>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindById(int id)
    {
        try
        {
            var response = await _service.FindRoleByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Role>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[AllowAnonymous]
    public async Task<IActionResult> UpdateRole([FromBody] RoleUpdate update, int id)
    {
        try
        {
            var response = await _service.UpdateRoleAsync(id, update);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }



    [HttpDelete("delete-permanent/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Role>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[AllowAnonymous]
    public async Task<IActionResult> HardDeleteRole(int id)
    {
        try
        {
            var response = await _service.HardDeleteRoleAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }
}
