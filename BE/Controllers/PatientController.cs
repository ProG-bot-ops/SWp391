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

        [HttpGet("search")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchPatient([FromQuery] string? phone = null, [FromQuery] string? cccd = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(cccd))
                {
                    return BadRequest("Vui lòng cung cấp số điện thoại hoặc CCCD để tìm kiếm");
                }

                var patient = await _service.SearchPatientByPhoneOrCCCDAsync(phone, cccd);
                if (patient is null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy bệnh nhân" });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        id = patient.Id,
                        name = patient.Name ?? "N/A",
                        phone = patient.Phone ?? "N/A",
                        cccd = patient.CCCD ?? "N/A",
                        email = patient.User?.Email ?? "N/A",
                        address = patient.Address ?? "N/A",
                        birthDate = patient.Dob.ToString("yyyy-MM-dd"),
                        gender = patient.Gender.ToString(),
                        imageUrl = patient.ImageURL ?? "N/A",
                        status = patient.Status.ToString()
                    },
                    message = "Tìm kiếm bệnh nhân thành công"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchPatient: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatePatient([FromBody] PatientCreateRequest request)
        {
            try
            {
                var result = await _service.CreatePatientOnlyAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("create-with-appointment")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatePatientWithAppointment([FromBody] PatientAndAppointmentCreateRequest request)
        {
            try
            {
                var result = await _service.CreatePatientAndAppointmentAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
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

        [HttpGet("debug-search")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DebugSearch([FromQuery] string? phone = null, [FromQuery] string? cccd = null)
        {
            try
            {
                var result = await _service.DebugSearchAsync(phone, cccd);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("list-all")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListAllPatients()
        {
            try
            {
                var allPatients = await _service.GetAllPatientsForDebugAsync();
                return Ok(allPatients);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("sequence-info")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSequenceInfo()
        {
            try
            {
                var result = await _service.GetPatientSequenceInfoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("reset-auto-increment")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ResetAutoIncrement()
        {
            try
            {
                var result = await _service.ResetPatientAutoIncrementAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
