using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service.Impl;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineManagerForAdminController : ControllerBase
    {
        private readonly IMedicineManageForAdminService _medicineService;

        public MedicineManagerForAdminController(IMedicineManageForAdminService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpGet("MedicineManage")]
        [ProducesResponseType(typeof(IEnumerable<MedicineManageForAdminDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllMedicineInventory()
        {
            try
            {
                var response = await _medicineService.GetAllInfoMedicineInventoryAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

    }
}
