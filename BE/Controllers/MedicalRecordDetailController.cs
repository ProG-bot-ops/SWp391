using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicalRecord;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using System;
using System.Net;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [ApiController]
    [Route("api/medical-record")]
    public class MedicalRecordDetailController : ControllerBase
    {
        private readonly IMedicalRecordDetailService _detailService;
        private readonly ILogger<MedicalRecordDetailController> _logger;

        public MedicalRecordDetailController(IMedicalRecordDetailService detailService, ILogger<MedicalRecordDetailController> logger)
        {
            _detailService = detailService;
            _logger = logger;
        }

        /// [VIEW DETAIL] Lấy thông tin chi tiết một Medical Record theo ID.
        [HttpGet("view/{medicalRecordId}")]
        [ProducesResponseType(typeof(MedicalRecordDetailResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult GetMedicalRecordDetail(int medicalRecordId)
        {
            try
            {
                var detail = _detailService.GetMedicalRecordDetail(medicalRecordId);
                if (detail == null)
                    return NotFound($"Không tìm thấy Medical Record với ID: {medicalRecordId}");

                return Ok(detail);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy thông tin chi tiết Medical Record.");
                return StatusCode(500, "Đã xảy ra lỗi khi lấy chi tiết Medical Record");
            }
        }

        /// [CREATE] Tạo mới một Medical Record.
        [HttpPost("create")]
        [ProducesResponseType(typeof(MedicalRecordDetailResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult CreateMedicalRecord([FromBody] MedicalRecordCreateRequest request)
        {
            try
            {
                var result = _detailService.CreateMedicalRecord(request);
                return CreatedAtAction(nameof(GetMedicalRecordDetail), new { medicalRecordId = result.Id }, result);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi tạo mới Medical Record.");
                return StatusCode(500, "Đã xảy ra lỗi khi tạo mới Medical Record");
            }
        }

        /// [UPDATE] Cập nhật thông tin một Medical Record theo ID.
        [HttpPut("update/{medicalRecordId}")]
        [ProducesResponseType(typeof(MedicalRecordDetailResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult UpdateMedicalRecord(int medicalRecordId, [FromBody] MedicalRecordUpdateRequest request)
        {
            try
            {
                var result = _detailService.UpdateMedicalRecord(medicalRecordId, request);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi cập nhật Medical Record.");
                return StatusCode(500, "Đã xảy ra lỗi khi cập nhật Medical Record");
            }
        }

        /// [DELETE] Xóa một Medical Record theo ID.
        [HttpDelete("delete/{medicalRecordId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult DeleteMedicalRecord(int medicalRecordId)
        {
            try
            {
                var success = _detailService.DeleteMedicalRecord(medicalRecordId);
                if (!success)
                    return NotFound($"Không tìm thấy Medical Record với ID: {medicalRecordId}");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi xóa Medical Record.");
                return StatusCode(500, "Đã xảy ra lỗi khi xóa Medical Record");
            }
        }
    }
}