using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineInventoryController : ControllerBase
    {
        private readonly IMedicineInventoryService _service;

        public MedicineInventoryController(IMedicineInventoryService service)
        {
            _service = service;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(MedicineInventoryPageDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("PageNumber và PageSize phải lớn hơn 0.");
            }

            try
            {
                var result = await _service.GetPagedAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<MedicineInventoryResponseDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string keyword = "", [FromQuery] string sortBy = "", [FromQuery] bool ascending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _service.SearchAsync(keyword, sortBy, ascending,pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
