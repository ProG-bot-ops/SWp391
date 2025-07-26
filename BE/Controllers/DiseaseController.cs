using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Disease;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using System;
using System.Collections.Generic;
using System.Net;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiseaseController : ControllerBase
    {
        private readonly IDiseaseService _diseaseService;
        private readonly ILogger<DiseaseController> _logger;

        public DiseaseController(IDiseaseService diseaseService, ILogger<DiseaseController> logger)
        {
            _diseaseService = diseaseService;
            _logger = logger;
        }

        /// Lấy danh sách tất cả các bệnh
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<DiseaseResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult GetAllDiseases()
        {
            try
            {
                var diseases = _diseaseService.GetAllDiseases();
                return Ok(diseases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy danh sách bệnh");
                return StatusCode(500, "Đã xảy ra lỗi khi lấy danh sách bệnh");
            }
        }

        /// Lấy thông tin chi tiết một bệnh theo ID
        [HttpGet("find-id/{id}")]
        [ProducesResponseType(typeof(DiseaseResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult GetDiseaseById(int id)
        {
            try
            {
                var disease = _diseaseService.GetDiseaseDetail(id);
                if (disease == null)
                    return NotFound($"Không tìm thấy bệnh với ID: {id}");

                return Ok(disease);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi lấy thông tin bệnh");
                return StatusCode(500, "Đã xảy ra lỗi khi lấy thông tin bệnh");
            }
        }

        /// Tạo mới một bệnh
        [HttpPost("add-disease")]
        [ProducesResponseType(typeof(DiseaseResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult CreateDisease([FromBody] DiseaseCreateRequest request)
        {
            try
            {
                var createdDisease = _diseaseService.CreateDisease(request);
                return CreatedAtAction(nameof(GetDiseaseById), new { id = createdDisease.Id }, createdDisease);
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
                _logger.LogError(ex, "Đã xảy ra lỗi khi tạo mới bệnh");
                return StatusCode(500, "Đã xảy ra lỗi khi tạo mới bệnh");
            }
        }

        /// Cập nhật thông tin một bệnh
        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(DiseaseResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult UpdateDisease(int id, [FromBody] DiseaseUpdateRequest request)
        {
            try
            {
                var updatedDisease = _diseaseService.UpdateDisease(id, request);
                return Ok(updatedDisease);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("Không tìm thấy"))
                    return NotFound(ex.Message);

                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi cập nhật bệnh");
                return StatusCode(500, "Đã xảy ra lỗi khi cập nhật bệnh");
            }
        }

        /// Xóa một bệnh theo ID
        [HttpDelete("delete-disease/{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public IActionResult DeleteDisease(int id)
        {
            try
            {
                var result = _diseaseService.DeleteDisease(id);
                if (!result)
                    return NotFound($"Không tìm thấy bệnh với ID: {id} hoặc bệnh đang được sử dụng trong hồ sơ y tế");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu đầu vào không hợp lệ: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi xóa bệnh");
                return StatusCode(500, "Đã xảy ra lỗi khi xóa bệnh");
            }
        }
    }
}