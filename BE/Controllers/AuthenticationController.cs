using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authenication;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authentication;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using SWP391_SE1914_ManageHospital.Service;
using System.Security.Claims;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _service;
    private readonly INurseService _nurseService;
    private readonly IDoctorService _doctorService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthService service, ILogger<AuthenticationController> logger, INurseService nurseService, IDoctorService doctorService)
    {
        _service = service;
        _logger = logger;
        _nurseService = nurseService;
        _doctorService = doctorService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        Console.WriteLine($"Received login request: Email={request.Email}, UserType={request.UserType}");

        try
        {
            var response = await _service.LoginAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.ToString());
        }
    }
    [HttpPost("register/patient")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterPatientAsync([FromBody] PatientRegisterRequest request)
    {
        try
        {
            Console.WriteLine($"=== PATIENT REGISTRATION REQUEST RECEIVED ===");
            Console.WriteLine($"Request body: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            // Debug logging
            Console.WriteLine($"=== PATIENT REGISTRATION DEBUG ===");
            Console.WriteLine($"Email: {request.Email}");
            Console.WriteLine($"FullName: {request.FullName}");
            Console.WriteLine($"Code: {request.Code}");
            Console.WriteLine($"Gender: {request.Gender}");
            Console.WriteLine($"Dob: {request.Dob}");
            Console.WriteLine($"CCCD: {request.CCCD}");
            Console.WriteLine($"Phone: {request.Phone}");
            Console.WriteLine($"EmergencyContact: {request.EmergencyContact}");
            Console.WriteLine($"Address: {request.Address}");
            
            // Validation
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { success = false, message = "Email không được để trống" });
                
            if (string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Mật khẩu không được để trống" });
                
            if (string.IsNullOrEmpty(request.FullName))
                return BadRequest(new { success = false, message = "Họ tên không được để trống" });
                
            if (string.IsNullOrEmpty(request.CCCD))
                return BadRequest(new { success = false, message = "CCCD không được để trống" });
                
            if (string.IsNullOrEmpty(request.Phone))
                return BadRequest(new { success = false, message = "Số điện thoại không được để trống" });
                
            if (string.IsNullOrEmpty(request.EmergencyContact))
                return BadRequest(new { success = false, message = "Liên hệ khẩn cấp không được để trống" });
                
            if (string.IsNullOrEmpty(request.Address))
                return BadRequest(new { success = false, message = "Địa chỉ không được để trống" });
            
            Console.WriteLine($"=== CALLING SERVICE ===");
            var response = await _service.RegisterPatientAsync(request);
            Console.WriteLine($"=== SERVICE RESPONSE ===");
            Console.WriteLine($"Response: {System.Text.Json.JsonSerializer.Serialize(response)}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== PATIENT REGISTRATION ERROR ===");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            _logger.LogError(ex, "Lỗi khi tạo tài khoản bệnh nhân!");
            return BadRequest(new { 
                success = false, 
                message = "Lỗi khi đăng ký tài khoản bệnh nhân", 
                error = ex.Message 
            });
        }
    }

    [HttpGet("test")]
    [AllowAnonymous]
    public IActionResult Test()
    {
        return Ok(new { message = "Authentication controller is working", timestamp = DateTime.Now });
    }

    [HttpPost("test-login")]
    [AllowAnonymous]
    public IActionResult TestLogin([FromBody] LoginRequestDTO request)
    {
        try
        {
            Console.WriteLine($"Test login request received: Email={request.Email}, UserType={request.UserType}");
            
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { success = false, message = "Email is required" });
                
            if (string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Password is required" });
            
            return Ok(new { 
                success = true, 
                message = "Test login request received successfully",
                data = new { email = request.Email, userType = request.UserType.ToString() }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test login error: {ex.Message}");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
    [HttpPost("register/nurse")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterNurseAsync([FromBody] NurseRegisterRequest request)
    {
        try
        {
            var response = await _nurseService.NurseRegisterAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tạo tài khoản y tá lỗi!");
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("register/doctor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterDoctorAsync([FromBody] DoctorRegisterRequest request)
    {
        try
        {
            var response = await _doctorService.DoctorRegisterAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tạo tài khoản bac si lỗi!");
            return BadRequest(ex.ToString());
        }
    }
    [HttpPost("change-password")]
    [Authorize] // Vẫn yêu cầu đăng nhập
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("UserType")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !int.TryParse(userIdClaim, out int userId) ||
                !Enum.TryParse<UserType>(userTypeClaim, out UserType userType))
            {
                return Unauthorized("Token không hợp lệ.");
            }

            var result = await _service.ChangePasswordAsync(
                userId,
                userType,
                request.OldPassword,
                request.NewPassword,
                request.ConfirmPassword
            );

            return Ok("Đổi mật khẩu thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đổi mật khẩu.");
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
    {
        try
        {
            await _service.ForgotPasswordAsync(request.Email, request.UserType);
            return Ok("Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi quên mật khẩu");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
    {
        try
        {
            await _service.ResetPasswordAsync(request.Token, request.NewPassword, request.UserType);
            return Ok("Đặt lại mật khẩu thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu");
            return BadRequest(new { message = "Đặt lại mật khẩu không thành công" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("UserType")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !int.TryParse(userIdClaim, out int userId) ||
                !Enum.TryParse<UserType>(userTypeClaim, out UserType userType))
            {
                return Unauthorized("Token không hợp lệ.");
            }

            var result = await _service.LogoutAsync(userId, userType);
            
            if (result)
            {
                return Ok(new { success = true, message = "Đăng xuất thành công" });
            }
            else
            {
                return BadRequest(new { success = false, message = "Đăng xuất thất bại" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng xuất");
            return BadRequest(new { success = false, message = "Lỗi khi đăng xuất: " + ex.Message });
        }
    }
}
