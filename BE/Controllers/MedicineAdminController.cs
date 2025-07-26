using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineAdmin;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineAdminController : ControllerBase
    {
        private readonly IMedicineAdminService _service;
        private readonly ICloudinaryService _cloudinaryService;
        public MedicineAdminController(IMedicineAdminService service, ICloudinaryService cloudinaryService)
        {
            _service = service;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<MedicineAdmin>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<ActionResult<IEnumerable<MedicineAdmin>>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var res = await _service.GetMedicineAsync(pageNumber, pageSize);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<MedicineAdmin>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchKeyword([FromQuery] string keyword ="",
                                                        [FromQuery] decimal? startPrice = null,
                                                        [FromQuery] decimal? endPrice = null,
                                                        [FromQuery] int pageNumber = 1, 
                                                        [FromQuery] int pageSize = 10)
        {
            try
            {
                var res = await _service.Search(keyword, startPrice, endPrice,pageNumber, pageSize);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(IEnumerable<MedicineAdmin>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateMedicineInfo([FromBody] MedicineAdminUpdate up, int id)
        {
            try
            {
                
                var res = await _service.UpdateMedicineAsync(id, up);

                if (!res)
                {
                   
                    return BadRequest("Không thể cập nhật thuốc. Vui lòng kiểm tra lại thông tin.");
                }

               
                return Ok(res);
            }
            catch (Exception ex)
            {
                
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        [HttpPut("update-image/{id}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateMedicineImage(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var imageUrl = await _cloudinaryService.UploadImageAsync(file);
 
                var res = await _service.UpdateMedicineImageAsync(id, imageUrl);

                if (!res)
                {
                    return BadRequest("Không thể cập nhật ảnh thuốc. Vui lòng thử lại.");
                }

                return Ok("Ảnh đã được cập nhật thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }
}

