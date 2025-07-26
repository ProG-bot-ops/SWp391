using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineDetailController : ControllerBase
    {
        private readonly IMedicineDetailService _medicineDetailService;

        public MedicineDetailController(IMedicineDetailService medicineDetailService)
        {
            _medicineDetailService = medicineDetailService;
        }

        // Lấy tất cả MedicineDetails
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _medicineDetailService.GetAllAsync();
            return Ok(result);
        }

        // Lấy MedicineDetail theo ID
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _medicineDetailService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // Tạo mới MedicineDetail
        [HttpPost("add-medicine-detail")]
        public async Task<IActionResult> Create([FromBody] MedicineDetailRequest request)
        {
            var result = await _medicineDetailService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // Cập nhật MedicineDetail
        [HttpPut("update-medicine-detail/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicineDetailRequest request)
        {
            var result = await _medicineDetailService.UpdateAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // Xóa MedicineDetail
        [HttpDelete("delete-medicine-detail/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _medicineDetailService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // Tìm kiếm MedicineDetail theo thành phần (Ingredients)
        [HttpGet("search-by-ingredients")]
        public async Task<IActionResult> SearchByIngredients(string ingredients)
        {
            if (string.IsNullOrWhiteSpace(ingredients))
            {
                return BadRequest("Ingredients cannot be empty.");
            }

            var result = await _medicineDetailService.SearchByIngredientsAsync(ingredients);
            return Ok(result);
        }
    }
}