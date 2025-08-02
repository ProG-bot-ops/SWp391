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
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

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

        // API mới: Lấy danh sách tất cả bệnh nhân
        [HttpGet("list")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllPatients()
        {
            try
            {
                var patients = await _service.GetAllPatientAsync();
                var patientList = patients.Select(p => new
                {
                    id = p.Id,
                    code = p.Code,
                    name = p.Name,
                    cccd = p.CCCD,
                    phone = p.Phone,
                    dob = p.Dob.ToString("yyyy-MM-dd"),
                    gender = p.Gender,
                    address = p.Address,
                    bloodType = p.BloodType,
                    insuranceNumber = p.InsuranceNumber,
                    allergies = p.Allergies,
                    status = p.Status,
                    createDate = p.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    updateDate = p.UpdateDate?.ToString("yyyy-MM-dd HH:mm:ss")
                });

                return Ok(new
                {
                    success = true,
                    data = patientList,
                    message = "Lấy danh sách bệnh nhân thành công"
                });
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

        // API mới: Tạo bệnh nhân mới với validation đầy đủ
        [HttpPost("add")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddPatient([FromBody] PatientCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

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

        [HttpPost("add-with-image")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddPatientWithImage([FromForm] IFormCollection form)
        {
            try
            {
                // Parse form data
                var fullName = form["fullName"].ToString();
                var cccd = form["cccd"].ToString();
                var phone = form["phone"].ToString();
                var dob = form["dob"].ToString();
                var gender = form["gender"].ToString();
                var address = form["address"].ToString();
                var bloodType = form["bloodType"].ToString();
                var insuranceNumber = form["insuranceNumber"].ToString();
                var allergies = form["allergies"].ToString();
                var imageFile = form.Files.FirstOrDefault(f => f.Name == "imageFile");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    return BadRequest(new { success = false, message = "Tên bệnh nhân không được để trống" });
                }
                if (string.IsNullOrWhiteSpace(cccd) || cccd.Length != 12)
                {
                    return BadRequest(new { success = false, message = "CCCD phải có đúng 12 số" });
                }
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return BadRequest(new { success = false, message = "Số điện thoại không được để trống" });
                }
                if (string.IsNullOrWhiteSpace(dob))
                {
                    return BadRequest(new { success = false, message = "Ngày sinh không được để trống" });
                }
                if (string.IsNullOrWhiteSpace(gender))
                {
                    return BadRequest(new { success = false, message = "Giới tính không được để trống" });
                }
                if (string.IsNullOrWhiteSpace(address))
                {
                    return BadRequest(new { success = false, message = "Địa chỉ không được để trống" });
                }

                // Create PatientCreateRequest
                var request = new PatientCreateRequest
                {
                    FullName = fullName,
                    CCCD = cccd,
                    Phone = phone,
                    Dob = DateTime.Parse(dob),
                    Gender = int.Parse(gender),
                    Address = address,
                    BloodType = string.IsNullOrWhiteSpace(bloodType) ? null : bloodType,
                    InsuranceNumber = string.IsNullOrWhiteSpace(insuranceNumber) ? null : insuranceNumber,
                    Allergies = string.IsNullOrWhiteSpace(allergies) ? null : allergies
                };

                // Create patient first
                object result;
                try
                {
                    result = await _service.CreatePatientOnlyAsync(request);
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Lỗi khi tạo bệnh nhân: {ex.Message}",
                        details = ex.InnerException?.Message
                    });
                }
                
                // If image file is provided, save it
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Validate file size (max 5MB)
                    if (imageFile.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest(new { success = false, message = "Kích thước file ảnh không được vượt quá 5MB" });
                    }

                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(imageFile.ContentType.ToLower()))
                    {
                        return BadRequest(new { success = false, message = "Chỉ chấp nhận file ảnh định dạng JPG, PNG, GIF" });
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadPath = Path.Combine(wwwrootPath, "uploads", "patients");
                    
                    // Create wwwroot directory if it doesn't exist
                    if (!Directory.Exists(wwwrootPath))
                    {
                        Directory.CreateDirectory(wwwrootPath);
                    }
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Update patient with image URL
                    var imageUrl = $"uploads/patients/{fileName}";
                    // Get the patient ID from the result - simplified approach
                    var resultJson = System.Text.Json.JsonSerializer.Serialize(result);
                    var resultObj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(resultJson);
                    int patientId;
                    try
                    {
                        patientId = resultObj.GetProperty("data").GetProperty("id").GetInt32();
                    }
                    catch
                    {
                        // If we can't get the ID from result, try to find the patient by phone/cccd
                        var searchResult = await _service.SearchPatientByPhoneOrCCCDAsync(request.Phone, request.CCCD);
                        if (searchResult == null)
                        {
                            return BadRequest(new { success = false, message = "Không thể tìm thấy bệnh nhân vừa tạo" });
                        }
                        patientId = searchResult.Id;
                    }
                    
                    // Update the patient's image URL in database
                    var existingPatient = await _service.FindPatientByIdAsync(patientId);
                    if (existingPatient != null)
                    {
                        var updateRequest = new PatientUpdate
                        {
                            Name = existingPatient.Name,
                            Code = existingPatient.Code,
                            CCCD = existingPatient.CCCD,
                            Phone = existingPatient.Phone,
                            Dob = existingPatient.Dob,
                            Gender = existingPatient.Gender,
                            Address = existingPatient.Address,
                            BloodType = existingPatient.BloodType,
                            InsuranceNumber = existingPatient.InsuranceNumber,
                            Allergies = existingPatient.Allergies,
                            Status = existingPatient.Status,
                            EmergencyContact = existingPatient.EmergencyContact,
                            ImageURL = imageUrl,
                            UserId = existingPatient.UserId,
                            UpdateBy = "System"
                        };

                        try
                        {
                            await _service.UpdatePatientAsync(patientId, updateRequest);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = $"Lỗi khi cập nhật ảnh bệnh nhân: {ex.Message}",
                                details = ex.InnerException?.Message
                            });
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Thêm bệnh nhân thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    details = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // API mới: Cập nhật bệnh nhân với validation đầy đủ
        [HttpPut("edit/{id}")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EditPatient(int id, [FromBody] PatientUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

                // Kiểm tra xem bệnh nhân có tồn tại không
                var existingPatient = await _service.FindPatientByIdAsync(id);
                if (existingPatient == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy bệnh nhân"
                    });
                }

                // Cập nhật thông tin bệnh nhân
                var updateRequest = new PatientUpdate
                {
                    Name = request.Name,
                    Code = request.Code, // Giữ nguyên code
                    CCCD = request.CCCD,
                    Phone = request.Phone,
                    Dob = request.Dob,
                    Gender = request.Gender == -1 ? existingPatient.Gender : (Gender)request.Gender, // Giữ nguyên gender nếu gửi -1
                    Address = request.Address,
                    BloodType = request.BloodType,
                    InsuranceNumber = request.InsuranceNumber,
                    Allergies = request.Allergies,
                    Status = request.Status?.ToLower() == "banned" ? PatientStatus.Banned : PatientStatus.Active,
                    EmergencyContact = "", // Để trống nếu không có
                    ImageURL = "", // Để trống nếu không có
                    UserId = 0, // Không thay đổi UserId
                    UpdateBy = "System"
                };

                var result = await _service.UpdatePatientAsync(id, updateRequest);
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Cập nhật bệnh nhân thành công"
                });
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

        // API mới: Lấy chi tiết bệnh nhân
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPatientDetail(int id)
        {
            try
            {
                var patient = await _service.FindPatientByIdAsync(id);
                if (patient == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy bệnh nhân"
                    });
                }

                // Log để debug
                Console.WriteLine($"Patient ID: {patient.Id}");
                Console.WriteLine($"Patient ImageURL: {patient.ImageURL}");
                
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        id = patient.Id,
                        code = patient.Code,
                        name = patient.Name,
                        cccd = patient.CCCD,
                        phone = patient.Phone,
                        dob = patient.Dob.ToString("yyyy-MM-dd"),
                        gender = patient.Gender,
                        address = patient.Address,
                        bloodType = patient.BloodType,
                        insuranceNumber = patient.InsuranceNumber,
                        allergies = patient.Allergies,
                        status = patient.Status,
                        imageURL = patient.ImageURL,
                        createDate = patient.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        updateDate = patient.UpdateDate?.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    message = "Lấy thông tin bệnh nhân thành công"
                });
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

        // API mới: Cập nhật ảnh bệnh nhân
        [HttpPut("update-image/{id}")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdatePatientImage(int id, IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vui lòng chọn file ảnh"
                    });
                }

                // Kiểm tra kích thước file (max 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Kích thước file không được vượt quá 5MB"
                    });
                }

                // Kiểm tra định dạng file
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(imageFile.ContentType.ToLower()))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Chỉ chấp nhận file ảnh định dạng JPG, PNG, GIF"
                    });
                }

                // Tìm bệnh nhân
                var patient = await _service.FindPatientByIdAsync(id);
                if (patient == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy bệnh nhân"
                    });
                }

                // Tạo tên file unique
                var fileName = $"patient_{id}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(imageFile.FileName)}";
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadPath = Path.Combine(wwwrootPath, "uploads", "patients");
                
                // Tạo thư mục wwwroot nếu chưa tồn tại
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }
                
                // Tạo thư mục uploads/patients nếu chưa tồn tại
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn ảnh trong database - sử dụng đường dẫn tương đối đúng
                var imageUrl = $"uploads/patients/{fileName}";
                
                // Log để debug
                Console.WriteLine($"Image saved to: {filePath}");
                Console.WriteLine($"Image URL: {imageUrl}");
                
                // Tạo update request với đầy đủ thông tin patient
                var updateRequest = new PatientUpdate
                {
                    Name = patient.Name,
                    Code = patient.Code,
                    CCCD = patient.CCCD,
                    Phone = patient.Phone,
                    Dob = patient.Dob,
                    Gender = patient.Gender,
                    Address = patient.Address,
                    BloodType = patient.BloodType,
                    InsuranceNumber = patient.InsuranceNumber,
                    Allergies = patient.Allergies,
                    Status = patient.Status,
                    EmergencyContact = patient.EmergencyContact,
                    ImageURL = imageUrl,
                    UserId = patient.UserId,
                    UpdateBy = "System"
                };
                
                var result = await _service.UpdatePatientAsync(id, updateRequest);

                return Ok(new
                {
                    success = true,
                    data = new { imageUrl = imageUrl },
                    message = "Cập nhật ảnh bệnh nhân thành công"
                });
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

        // API mới: Xóa bệnh nhân (soft delete)
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var result = await _service.SoftDeletePatientColorAsync(id, PatientStatus.Banned);
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Xóa bệnh nhân thành công"
                });
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

        // API mới: Tìm kiếm bệnh nhân theo tên
        [HttpGet("search/name/{name}")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchPatientsByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên bệnh nhân không được để trống"
                    });
                }

                var patients = await _service.SearchPatientByKeyAsync(name);
                var patientList = patients.Select(p => new
                {
                    id = p.Id,
                    code = p.Code,
                    name = p.Name,
                    cccd = p.CCCD,
                    phone = p.Phone,
                    dob = p.Dob.ToString("yyyy-MM-dd"),
                    gender = p.Gender,
                    address = p.Address,
                    bloodType = p.BloodType,
                    insuranceNumber = p.InsuranceNumber,
                    allergies = p.Allergies,
                    status = p.Status
                });

                return Ok(new
                {
                    success = true,
                    data = patientList,
                    message = "Tìm kiếm bệnh nhân thành công"
                });
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
