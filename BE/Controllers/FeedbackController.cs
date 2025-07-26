using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using System.Net;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Feedback;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _service;

    public FeedbackController(IFeedbackService service)
    {
        _service = service;
    }

    [HttpPost("add-feedback")]
    [ProducesResponseType(typeof(IEnumerable<Feedback>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddFeedback([FromBody] FeedbackCreate create)
    {
        try
        {
            var response = await _service.CreateFeedbackAsync(create);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("get-all")]
    [ProducesResponseType(typeof(IEnumerable<Feedback>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedback()
    {
        try
        {
            var response = await _service.GetAllFeedbackAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-by-name")]
    [ProducesResponseType(typeof(IEnumerable<Feedback>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindByPatientName(string? name, DateTime? appointmentDate, DateTime? startTime)
    {
        try
        {
            var response = await _service.SearchFeedbackByFilterAsync(name, appointmentDate, startTime);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("find-id/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Feedback>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindByPatientId(int id)
    {
        try
        {
            var response = await _service.FindFeedbackByPatientIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    
}
