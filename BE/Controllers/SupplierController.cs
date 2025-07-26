using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Supplier;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;
namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;
        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpPost("add-supplier")]
        [ProducesResponseType(typeof(IEnumerable<Supplier>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierCreate supplier)
        {
            try
            {
                var response = await _service.CreateSupplerAsync(supplier);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<Supplier>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSupplier()
        {
            try
            {
                var response = await _service.GetAllSupplierAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("find-by-name/{name}")]
        [ProducesResponseType(typeof(IEnumerable<Supplier>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByName(string name)
        {
            try
            {
                var response = await _service.SearchSupplierByKeyAsync(name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Supplier>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateSupplier([FromBody] SupplierUpdate supplier, int id)
        {
            try
            {
                var result = await _service.UpdateSupplerAsync(id, supplier);
                if (result == null)
                    return NotFound($"Supplier with ID {id} not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
