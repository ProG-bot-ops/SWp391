using System.Collections;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineImportController : ControllerBase
    {
        private readonly IMedicineImportService _service;

        public MedicineImportController(IMedicineImportService service)
        {
            _service = service;
        }

        [HttpPost("add-medicineimport")]
        [ProducesResponseType(typeof(IEnumerable<MedicineImport>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddMedicineImport([FromBody] MedicineImportCreate create)
        {
            try
            {
                var res = await _service.CreateMedicineImportAsync(create);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<MedicineImport>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<ActionResult<IEnumerable<MedicineImport>>> GetAllMedicineImport()
        {
            try
            {
                var respone = await _service.GetAllMedicineImportAsync();
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("find-by-name/{name}")]
        [ProducesResponseType(typeof(IEnumerable<MedicineImport>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByName(string name)
        {
            try
            {
                var res = await _service.SearchMedicineImportByName(name);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(IEnumerable<MedicineImport>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateMedicineImport( int id, [FromBody] MedicineImportUpdate update)
        {
            try
            {
                var res = await _service.UpdateMedicineImportAsync(id, update);
                if(res == null)
                {
                    return NotFound($"MedicineImport with ID {id} not found.");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }

    }
}
