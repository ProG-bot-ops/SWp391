using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
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
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        [HttpPost("AddPatient")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> AddPatient([FromBody] PatientCreate create)
        {
            try
            {
                var respone = await _service.CreatePatientAsync(create);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }


        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatient()
        {
            try
            {
                var response = await _service.GetAllPatientAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("FindByName/{name}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByName(string name)
        {
            try
            {
                var response = await _service.SearchPatientByKeyAsync(name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("findId/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindById(int id)
        {
            try
            {
                var response = await _service.FindPatientByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("findUserId/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByUserId(int userId)
        {
            try
            {
                var response = await _service.FindPatientByUserIdAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdatePatient([FromBody] PatientUpdate update, int id)
        {
            try
            {
                var respone = await _service.UpdatePatientAsync(id, update);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());  
            }
            

        }

        [HttpPut("updateByUserId/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdatePatientbyUserId([FromBody] PatientUpdate update, int userId)
        {
            try
            {
                var respone = await _service.UpdatePatientByUserIdAsync(userId, update);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut("updatePatientImage/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdatePatientImage(int userId, [FromBody] PatientImageUpdate imageURL)
        {
            try
            {
                var respone = await _service.UpdatePatientImageAsync(userId, imageURL);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut("change-status/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> SoftDeletePatient(int id, PatientStatus newStatus)
        {
            try
            {
                var respone = await _service.SoftDeletePatientColorAsync(id, newStatus);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("delete-patient/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> HardDeletePatient(int id)
        {
            try
            {
                var respone = await _service.HardDeletePatientAsync(id);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }

        [HttpGet("PatientInfoAd")]
        [ProducesResponseType(typeof(IEnumerable<PatientInfoAdmin>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<PatientInfoAdmin>>> GetAllPatientBillingHistory()
        {
            try
            {
                var response = await _service.PatientInfoAdAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}
