using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWP391_SE1914_ManageHospital.Service;
using System.Linq;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [ApiController]
    [Route("api/medical-records")]
    public class MedicalRecordListController : ControllerBase
    {
        private readonly IMedicalRecordListService _listService;
        private readonly ILogger<MedicalRecordListController> _logger;

        public MedicalRecordListController(IMedicalRecordListService listService, ILogger<MedicalRecordListController> logger)
        {
            _listService = listService;
            _logger = logger;
        }

        [HttpGet("patient/{patientId}")]
        public IActionResult GetMedicalRecordsByPatientId(int patientId)
        {
            try
            {
                _logger.LogInformation("Đang lấy danh sách medical records cho bệnh nhân ID: {PatientId}", patientId);
                var records = _listService.GetMedicalRecordsByPatientId(patientId);

                // Tạo đối tượng response bao gồm tổng số bản ghi và danh sách
                var response = new
                {
                    TotalCount = records.Count(),
                    Records = records
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy hồ sơ bệnh án cho ID bệnh nhân {PatientId}", patientId);
                return StatusCode(500, "Đã xảy ra lỗi khi lấy danh sách Medical Records");
            }
        }
    }
}