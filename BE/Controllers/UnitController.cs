using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;

namespace SWP391_SE1914_ManageHospital.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _service;

        public UnitController(IUnitService service)
        {
            _service = service;
        }


        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<Unit>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<Unit>>> GetAll()
        {
            try
            {
                var respone = await _service.GetAll();
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
