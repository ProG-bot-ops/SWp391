using Castle.Core.Resource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ClinicController : ControllerBase
{
    private readonly IClinicService _service;

    public ClinicController(IClinicService service)
    {
        _service = service;
    }

    [HttpPost("add-clinic")]
    [ProducesResponseType(typeof(IEnumerable<Clinic>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddClinic([FromBody] ClinicCreate create)
    {
        try
        {
            var response = await _service.CreateClinicAsync(create);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("get-all")]
    [ProducesResponseType(typeof(IEnumerable<Clinic>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Clinic>>> GetAllClinic()
    {
        try
        {
            var response = await _service.GetAllClinicAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-by-name/{name}")]
    [ProducesResponseType(typeof(IEnumerable<Clinic>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindByName(string name)
    {
        try
        {
            var response = await _service.SearchClinicByKeyAsync(name);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-id/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Clinic>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindById(int id)
    {
        try
        {
            var response = await _service.FindClinicByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Clinic>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[AllowAnonymous]
    public async Task<IActionResult> UpdateClinic([FromBody] ClinicUpdate update, int id)
    {
        try
        {
            var response = await _service.UpdateClinicAsync(id, update);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Clinic>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[Authorize(Roles = "Admin, Employee")]
    public async Task<IActionResult> SoftDeleteClinic(int id, ClinicStatus newStatus)
    {
        try
        {
            var response = await _service.SoftDeleteClinicAsync(id, newStatus);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpDelete("delete-permanent/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Clinic>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    //[AllowAnonymous]
    public async Task<IActionResult> HardDeleteClinic(int id)
    {
        try
        {
            var response = await _service.HardDeleteClinicAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }
}
