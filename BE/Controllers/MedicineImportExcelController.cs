using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ImportMedicineEX;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using SWP391_SE1914_ManageHospital.Ultility;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineImportExcelController : ControllerBase
    {
        private readonly IMedicineImportExcelService _service;

        public MedicineImportExcelController(IMedicineImportExcelService service )
        {
            _service = service;
            
        }

        [HttpPost("import-excel-preview")]
        [ProducesResponseType(typeof(MedicineImportRequest), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ImportMedicinesPreview( IFormFile file, [FromForm] int supplierId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Vui lòng chọn file Excel để tải lên.");

                var previewRequest = await _service.ParseImportExcelToRequest(file, supplierId);
                return Ok(previewRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

  
        [HttpPost("confirm-import")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ConfirmImport([FromBody] MedicineImportRequest request)
        {
            try
            {
                var result = await _service.ConfirmImportAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
