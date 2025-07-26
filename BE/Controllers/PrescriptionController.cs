using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Prescription;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using System.Threading.Tasks;
using SWP391_SE1914_ManageHospital.Data;
using Microsoft.EntityFrameworkCore;
using static SWP391_SE1914_ManageHospital.Ultility.Status;


namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ApplicationDBContext _context;

        public PrescriptionController(IPrescriptionService prescriptionService, ApplicationDBContext context)
        {
            _prescriptionService = prescriptionService;
            _context = context;
        }

        // Lấy UserId từ Claims
        private int GetUserIdFromClaims()
        {
            var claim = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        // Kiểm tra người dùng có phải bác sĩ không
        private async Task<bool> IsDoctor(int userId)
        {
            var ur = await _context.User_Roles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (ur == null) return false;
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == ur.RoleId);
            return role != null && role.Name == "Doctor";
        }

        // Lấy tên người dùng từ JWT claims
        private string GetUserNameFromClaims()
        {
            return User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "system";
        }

        // Lấy tất cả đơn thuốc
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _prescriptionService.GetAllAsync();
            return Ok(list);
        }

        // Lấy đơn thuốc theo ID
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _prescriptionService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // Lấy đơn thuốc của bác sĩ theo DoctorId
        [HttpGet("get-by-doctor/{doctorId}")]
        [Authorize(Roles = "Doctor, Admin")]
        public async Task<IActionResult> GetByDoctor(int doctorId)
        {
            var list = await _prescriptionService.GetByDoctorIdAsync(doctorId);
            return Ok(list);
        }

        // Lấy đơn thuốc của bệnh nhân theo PatientId
        [HttpGet("get-by-patient/{patientId}")]
        [Authorize(Roles = "Doctor, Admin")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var list = await _prescriptionService.GetByPatientIdAsync(patientId);
            return Ok(list);
        }

        // Lấy đơn thuốc của người dùng hiện tại
        [HttpGet("my-prescriptions")]
        [Authorize]
        public async Task<IActionResult> GetMine()
        {
            // Lấy UserId từ JWT token
            var userId = GetUserIdFromClaims();
            if (userId == 0) return Unauthorized("User chưa đăng nhập.");

            // Lấy role của người dùng từ claims
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role)) return Unauthorized("Role không hợp lệ.");

            // Lấy danh sách đơn thuốc của người dùng hiện tại
            var prescriptions = await _prescriptionService.GetMineAsync(userId, role);

            // Sắp xếp đơn thuốc theo ngày tạo (sắp xếp tăng dần, đơn thuốc mới sẽ ở cuối)
            prescriptions = prescriptions.OrderBy(p => p.CreateDate).ToList();

            return Ok(prescriptions); // Trả về danh sách đơn thuốc đã ánh xạ thông tin
        }




        // Thêm đơn thuốc mới
        [HttpPost("add-prescription")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create([FromBody] PrescriptionRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User chưa đăng nhập.");

            if (!await IsDoctor(userId))
                return Forbid("Chỉ bác sĩ mới có quyền tạo đơn thuốc.");

            request.UserId = userId;
            try
            {
                var created = await _prescriptionService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                // Kiểm tra lỗi do không tìm thấy bệnh nhân (nội dung giống bạn)
                if (ex.Message.Contains("Không tìm thấy bệnh nhân với thông tin cung cấp."))
                {
                    // Trả về lỗi gọn 1 dòng message
                    return BadRequest(new { message = "Không tìm thấy bệnh nhân với thông tin cung cấp." });
                }
                // Các lỗi khác: trả về lỗi mặc định hoặc log lại tùy nhu cầu
                return BadRequest(new { message = ex.Message });
            }
        }



        // Cập nhật đơn thuốc
        [HttpPut("update-prescription/{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] PrescriptionRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User chưa đăng nhập.");

            if (!await IsDoctor(userId))
                return Forbid("Chỉ bác sĩ mới có quyền cập nhật đơn thuốc.");

            request.UserId = userId;
            var updated = await _prescriptionService.UpdateAsync(id, request);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        // Xóa đơn thuốc
        [HttpDelete("delete-prescription/{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == 0)
                return Unauthorized("User chưa đăng nhập.");

            if (!await IsDoctor(userId))
                return Forbid("Chỉ bác sĩ mới có quyền xóa đơn thuốc.");

            var ok = await _prescriptionService.DeleteAsync(id);
            if (!ok)
                return NotFound();
            return NoContent();
        }

        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] PrescriptionUpdateStatusRequest request)
        {
            // Lấy userId từ token claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User chưa đăng nhập hoặc token không hợp lệ.");

            try
            {
                await _prescriptionService.UpdateStatusAsync(id, request.Status, userId);
                return Ok(new { Message = "Cập nhật trạng thái thành công." });
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (InvalidOperationException inv)
            {
                return BadRequest(inv.Message);
            }
        }
        
        [HttpGet("get-total/{prescriptionId}")]
        [Authorize(Roles = "Doctor,Admin,Patient")]
        public async Task<IActionResult> GetTotalAmount(int prescriptionId)
        {
            try
            {
                // Gọi đúng field đã inject
                var total = await _prescriptionService.GetTotalAmountAsync(prescriptionId);
                return Ok(new { prescriptionId, totalAmount = total });
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
        }


    }
}
