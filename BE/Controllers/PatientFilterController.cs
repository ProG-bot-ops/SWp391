using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PatientFilter;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientFilterController : ControllerBase
    {
        private readonly IPatientFilterService _patientService;
        private readonly ILogger<PatientFilterController> _logger;

        public PatientFilterController(IPatientFilterService patientService, ILogger<PatientFilterController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        [HttpPost("GetPatientSchedule")]
        public async Task<IActionResult> GetPatientSchedule([FromBody] PatientScheduleRequest request)
        {
            try
            {
                _logger.LogInformation($"=== GetPatientSchedule API Called ===");
                _logger.LogInformation($"DoctorId: {request.DoctorId}, Date: {request.Date}");

                var filter = new PatientFilter
                {
                    DoctorId = request.DoctorId,
                    PatientName = request.PatientName
                };

                var result = await _patientService.GetPatientsByDoctorAsync(filter);

                _logger.LogInformation($"=== Returning {result.Count} appointments ===");
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError($"ArgumentNullException: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"ArgumentException: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return StatusCode(500, new { message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        /// Lấy danh sách bệnh nhân của bác sĩ, có thể lọc theo ngày, giờ, tên bệnh nhân.
        [HttpPost("doctor-patients")]
        public async Task<IActionResult> GetPatientsByDoctor([FromBody] PatientFilter filter)
        {
            try
            {
                Console.WriteLine($"=== GetPatientsByDoctor API Called ===");
                Console.WriteLine($"DoctorId: {filter.DoctorId}");
                Console.WriteLine($"FromDate: {filter.FromDate}");
                Console.WriteLine($"ToDate: {filter.ToDate}");

                var result = await _patientService.GetPatientsByDoctorAsync(filter);

                Console.WriteLine($"=== Returning {result.Count} appointments ===");
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi xử lý yêu cầu: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// Lấy danh sách bệnh nhân của bác sĩ trong ngày hiện tại.
        [HttpGet("doctor/{doctorId}/today")]
        public async Task<IActionResult> GetTodayPatientsByDoctor(int doctorId)
        {
            try
            {
                var result = await _patientService.GetTodayPatientsByDoctorAsync(doctorId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi xử lý yêu cầu: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}