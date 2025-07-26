// Controllers/PrescriptionDetailController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PrescriptionDetail;
using SWP391_SE1914_ManageHospital.Service;
using System.Threading.Tasks;
using SWP391_SE1914_ManageHospital.Data;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]    // Yêu cầu login
    public class PrescriptionDetailController : ControllerBase
    {
        private readonly IPrescriptionDetailService _service;
        private readonly ApplicationDBContext _context; // Khai báo _context
        public PrescriptionDetailController(IPrescriptionDetailService service, ApplicationDBContext context)
        {
            _service = service;
            _context = context;  // Khởi tạo _context từ constructor
        }

        private int GetUserIdFromClaims()
        {
            var claim = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private string GetRoleFromClaims()
        {
            return User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                   ?? string.Empty;
        }

        private async Task<int> GetMedicineIdFromNameAsync(string medicineName)
        {
            if (string.IsNullOrEmpty(medicineName))
                return 0;  // Hoặc ném lỗi nếu tên thuốc không hợp lệ

            var medicine = await _context.Medicines
                .AsNoTracking()  // Không theo dõi thay đổi (chỉ để đọc)
                .FirstOrDefaultAsync(m => m.Name == medicineName);  // Tìm thuốc theo tên

            if (medicine == null)
                return 0;  // Hoặc ném một ngoại lệ nếu không tìm thấy thuốc

            return medicine.Id;  // Trả về ID thuốc
        }


        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // API để lấy MedicineId từ MedicineName
        [HttpGet("get-id-by-name/{medicineName}")]


        /// <summary>
        /// Lấy PrescriptionDetail của chính bác sĩ hoặc bệnh nhân đang login
        /// </summary>
        [HttpGet("my-details")]
        public async Task<IActionResult> GetMyDetails()
        {
            var userId = GetUserIdFromClaims();
            if (userId == 0) return Unauthorized();

            var role = GetRoleFromClaims();
            if (string.IsNullOrEmpty(role)) return Forbid();

            var list = await _service.GetByUserAsync(userId, role);
            return Ok(list);
        }

        [HttpGet("get-by-prescription/{prescriptionId}")]
        public async Task<IActionResult> GetByPrescription(int prescriptionId)
        {
            var list = await _service.GetByPrescriptionIdAsync(prescriptionId);
            return Ok(list);
        }



        // PrescriptionDetailController.cs
        [HttpPost("add-detail")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Create([FromBody] PrescriptionDetailRequest req)
        {
            var userId = GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User chưa đăng nhập.");

            try
            {
                var dto = await _service.CreateAsync(req, userId);
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }








        [HttpDelete("delete-detail/{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }


    }
}
