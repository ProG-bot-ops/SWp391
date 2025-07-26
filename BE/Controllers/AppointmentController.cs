using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment;

namespace SWP391_SE1914_ManageHospital.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly IClinicService _clinicService;
    private readonly IDoctorService _doctorService;
    private readonly IServiceService _serviceService;
    private readonly ApplicationDBContext _context;

    public AppointmentController(IClinicService clinicService, IDoctorService doctorService, IServiceService serviceService, ApplicationDBContext context)
    {
        _clinicService = clinicService;
        _doctorService = doctorService;
        _serviceService = serviceService;
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách phòng khám đang hoạt động (status = 0) để đặt lịch hẹn
    /// </summary>
    /// <returns>Danh sách phòng khám có thể đặt lịch</returns>
    [HttpGet("clinics")]
    [ProducesResponseType(typeof(IEnumerable<ClinicResponseDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetActiveClinics([FromQuery] DateTime date)
    {
        try
        {
            var clinics = await _clinicService.GetActiveClinicsForAppointmentAsync(date);
            return Ok(new
            {
                success = true,
                message = "Lấy danh sách phòng khám thành công",
                data = clinics
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                data = (object)null
            });
        }
    }

    /// <summary>
    /// Lấy danh sách bác sĩ theo phòng khám cho đặt lịch hẹn
    /// </summary>
    /// <param name="clinicId">ID phòng khám</param>
    /// <param name="date">Ngày đặt lịch</param>
    /// <returns>Danh sách bác sĩ có thể đặt lịch</returns>
    [HttpGet("doctors/{clinicId}")]
    [ProducesResponseType(typeof(IEnumerable<DoctorResponseDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDoctorsByClinic(int clinicId, [FromQuery] DateTime date)
    {
        try
        {
            var doctors = await _doctorService.GetDoctorsByClinicIdAsync(clinicId, date);
            return Ok(new
            {
                success = true,
                message = "Lấy danh sách bác sĩ thành công",
                data = doctors
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                data = (object)null
            });
        }
    }

    /// <summary>
    /// Tìm kiếm phòng khám theo tên cho đặt lịch hẹn
    /// </summary>
    /// <param name="name">Tên phòng khám cần tìm</param>
    /// <returns>Danh sách phòng khám phù hợp</returns>
    [HttpGet("clinics/search")]
    [ProducesResponseType(typeof(IEnumerable<ClinicResponseDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SearchClinics([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Tên phòng khám không được để trống",
                    data = (object)null
                });
            }

            var clinics = await _clinicService.SearchClinicByKeyAsync(name);
            return Ok(new
            {
                success = true,
                message = "Tìm kiếm phòng khám thành công",
                data = clinics
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                data = (object)null
            });
        }
    }

    /// <summary>
    /// Lấy thông tin chi tiết phòng khám theo ID
    /// </summary>
    /// <param name="id">ID phòng khám</param>
    /// <returns>Thông tin chi tiết phòng khám</returns>
    [HttpGet("clinics/{id}")]
    [ProducesResponseType(typeof(ClinicResponseDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetClinicById(int id)
    {
        try
        {
            var clinic = await _clinicService.FindClinicByIdAsync(id);
            return Ok(new
            {
                success = true,
                message = "Lấy thông tin phòng khám thành công",
                data = clinic
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                data = (object)null
            });
        }
    }

    [HttpGet("services-by-doctor/{doctorId:int}")]
    public async Task<IActionResult> GetServicesByDoctorId(int doctorId)
    {
        try
        {
            var departmentId = await _doctorService.GetDepartmentIdByDoctorIdAsync(doctorId);

            if (departmentId == null)
            {
                return Ok(new
                {
                    success = true,
                    message = "Bác sĩ không thuộc khoa nào, không có dịch vụ.",
                    data = new List<ServiceResponseDTO>()
                });
            }

            var services = await _serviceService.GetServicesByDepartmentAsync(departmentId.Value);

            return Ok(new
            {
                success = true,
                message = "Lấy danh sách dịch vụ thành công",
                data = services
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                data = (object)null
            });
        }
    }

    [HttpGet("available-shifts")]
    public async Task<IActionResult> GetAvailableShifts([FromQuery] int doctorId, [FromQuery] DateTime date)
    {
        // Định nghĩa thời gian ca sáng và chiều
        var morningStart = new TimeSpan(7, 30, 0);
        var morningEnd = new TimeSpan(12, 0, 0);
        var afternoonStart = new TimeSpan(13, 0, 0);
        var afternoonEnd = new TimeSpan(17, 30, 0);

        // Kiểm tra bác sĩ có làm ca sáng không
        var morningShift = await _context.Doctor_Shifts.FirstOrDefaultAsync(s => 
            s.DoctorId == doctorId && 
            s.ShiftDate.Date == date.Date && 
            s.ShiftType.ToLower() == "morning");

        var afternoonShift = await _context.Doctor_Shifts.FirstOrDefaultAsync(s => 
            s.DoctorId == doctorId && 
            s.ShiftDate.Date == date.Date && 
            s.ShiftType.ToLower() == "afternoon");

        // Đếm số lượng appointment đã đặt cho từng ca
        int morningCount = await _context.Appointments
            .Where(a => a.AppointmentDate.Date == date.Date
                && a.StartTime == morningStart
                && (a.EndTime.HasValue && a.EndTime.Value == morningEnd)
                && a.Doctor_Appointments.Any(da => da.DoctorId == doctorId))
            .CountAsync();

        int afternoonCount = await _context.Appointments
            .Where(a => a.AppointmentDate.Date == date.Date
                && a.StartTime == afternoonStart
                && (a.EndTime.HasValue && a.EndTime.Value == afternoonEnd)
                && a.Doctor_Appointments.Any(da => da.DoctorId == doctorId))
            .CountAsync();

        return Ok(new
        {
            morning = new
            {
                available = morningShift != null && morningCount < 5,
                count = morningCount,
                doctorWorks = morningShift != null
            },
            afternoon = new
            {
                available = afternoonShift != null && afternoonCount < 5,
                count = afternoonCount,
                doctorWorks = afternoonShift != null
            }
        });
    }

    [HttpGet("booked-time-slots")]
    public async Task<IActionResult> GetBookedTimeSlots([FromQuery] int doctorId, [FromQuery] string date)
    {
        // Debug: Log the raw input first
        Console.WriteLine($"=== GetBookedTimeSlots called ===");
        Console.WriteLine($"Raw input - DoctorId: {doctorId}, Date string: '{date}'");
        Console.WriteLine($"Date string length: {date?.Length}");
        if (!string.IsNullOrEmpty(date))
        {
            Console.WriteLine($"Date string bytes: {string.Join(", ", date.Select(c => (int)c))}");
        }

        try
        {
            // Check if date is null or empty
            if (string.IsNullOrEmpty(date))
            {
                Console.WriteLine("Date parameter is null or empty");
                return BadRequest(new
                {
                    success = false,
                    message = "Date parameter is required and cannot be empty",
                    data = (object)null
                });
            }

            // Try multiple date parsing approaches
            DateTime parsedDate = DateTime.MinValue;
            bool parseSuccess = false;

            // Try parsing with different formats
            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy/MM/dd", "dd-MM-yyyy", "MM-dd-yyyy" };
            
            Console.WriteLine($"Trying to parse date: '{date}'");
            foreach (string format in formats)
            {
                Console.WriteLine($"Trying format: {format}");
                if (DateTime.TryParseExact(date, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    Console.WriteLine($"Successfully parsed with format: {format}");
                    parseSuccess = true;
                    break;
                }
            }

            // If specific formats failed, try general parsing
            if (!parseSuccess)
            {
                Console.WriteLine("Trying general DateTime.TryParse");
                if (DateTime.TryParse(date, out parsedDate))
                {
                    Console.WriteLine("Successfully parsed with general DateTime.TryParse");
                    parseSuccess = true;
                }
            }

            if (!parseSuccess)
            {
                Console.WriteLine($"Failed to parse date: '{date}'");
                return BadRequest(new
                {
                    success = false,
                    message = $"Invalid date format: '{date}'. Expected format: yyyy-MM-dd",
                    data = (object)null
                });
            }

            // Debug log
            Console.WriteLine($"DoctorId: {doctorId}, Date string: '{date}', Parsed date: {parsedDate:yyyy-MM-dd}");

            // Lấy tất cả các time slot đã được đặt cho bác sĩ trong ngày cụ thể
            Console.WriteLine($"Querying appointments for date: {parsedDate:yyyy-MM-dd}");
            
            // Debug: Kiểm tra tất cả appointments trong ngày
            var allAppointmentsInDay = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == parsedDate.Date)
                .ToListAsync();
            Console.WriteLine($"Total appointments on {parsedDate:yyyy-MM-dd}: {allAppointmentsInDay.Count}");
            
            foreach (var apt in allAppointmentsInDay)
            {
                Console.WriteLine($"Appointment ID: {apt.Id}, Date: {apt.AppointmentDate:yyyy-MM-dd}, StartTime: {apt.StartTime}");
            }
            
            // Debug: Kiểm tra Doctor_Appointments
            var doctorAppointments = await _context.Doctor_Appointments
                .Where(da => da.DoctorId == doctorId)
                .ToListAsync();
            Console.WriteLine($"Total doctor appointments for doctor {doctorId}: {doctorAppointments.Count}");
            
            foreach (var da in doctorAppointments)
            {
                Console.WriteLine($"Doctor_Appointment - DoctorId: {da.DoctorId}, AppointmentId: {da.AppointmentId}");
            }
            
            // Lấy các time slots đã được đặt (chỉ lấy lịch hẹn chưa bị hủy)
            var bookedSlots = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == parsedDate.Date
                    && a.Doctor_Appointments.Any(da => da.DoctorId == doctorId)
                    && a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.StartTime)
                .ToListAsync();

            Console.WriteLine($"Found {bookedSlots.Count} booked slots");

            // Tạo danh sách các time slot đã được đặt với format HH:mm
            var bookedTimeSlots = new List<string>();
            foreach (var slot in bookedSlots)
            {
                try
                {
                    if (slot != null)
                    {
                        // Format TimeSpan thành HH:mm
                        var timeString = SafeTimeSpanToString(slot.Value);
                        bookedTimeSlots.Add(timeString);
                        Console.WriteLine($"Added time slot: {timeString}");
                    }
                }
                catch (Exception timeEx)
                {
                    Console.WriteLine($"Error formatting time slot: {slot}, Error: {timeEx.Message}");
                }
            }

            // Debug log
            Console.WriteLine($"Booked slots: {string.Join(", ", bookedTimeSlots)}");
            Console.WriteLine($"=== GetBookedTimeSlots completed successfully ===");

            return Ok(new
            {
                success = true,
                message = "Lấy danh sách slot đã đặt thành công",
                data = bookedTimeSlots
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== ERROR in GetBookedTimeSlots ===");
            Console.WriteLine($"Error message: {ex.Message}");
            Console.WriteLine($"Error type: {ex.GetType().Name}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Return a more specific error message
            string errorMessage = ex.Message;
            if (ex.Message.Contains("Input string was not in a correct format"))
            {
                errorMessage = $"Lỗi định dạng ngày tháng: '{date}'. Vui lòng kiểm tra lại định dạng yyyy-MM-dd";
            }
            
            return BadRequest(new
            {
                success = false,
                message = errorMessage,
                data = (object)null
            });
        }
    }

    [Authorize]
    [HttpGet("user-info")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
        if (user == null) return NotFound();
        var patient = await _context.Patients
            .Where(p => p.UserId == user.Id)
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync();
        var userData = new
        {
            id = user.Id,
            email = user.Email,
            name = patient?.Name,
            phone = patient?.Phone,
            gender = patient?.Gender,
            dob = patient?.Dob.ToString("yyyy-MM-dd"),
            cccd = patient?.CCCD,
            address = patient?.Address
        };

        return Ok(new
        {
            success = true,
            data = userData
        });
    }

    /// <summary>
    /// Tạo lịch hẹn mới
    /// </summary>
    /// <param name="request">Thông tin lịch hẹn</param>
    /// <returns>Thông tin lịch hẹn đã tạo</returns>
    [Authorize]
    [HttpPost("create")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CreateAppointment([FromBody] SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment.AppointmentCreateRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Dữ liệu yêu cầu không hợp lệ!" });
            if (request.AppointmentDate.Date < DateTime.Now.Date)
                return BadRequest(new { success = false, message = "Không thể đặt lịch cho ngày trong quá khứ!" });

            // Kiểm tra Shift có hợp lệ không
            if (string.IsNullOrEmpty(request.Shift) || (request.Shift.ToLower() != "morning" && request.Shift.ToLower() != "afternoon"))
                return BadRequest(new { success = false, message = "Ca làm việc không hợp lệ! Chỉ chấp nhận 'morning' hoặc 'afternoon'" });

            // Lấy userId từ token
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) 
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập để đặt lịch!" });
            if (!int.TryParse(userIdStr, out int userId)) 
                return Unauthorized(new { success = false, message = "Token không hợp lệ!" });

            // Lấy patient chính chủ (Id nhỏ nhất với UserId)
            var mainPatient = await _context.Patients
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Id)
                .FirstOrDefaultAsync();
            var patient = mainPatient;

            // --- Thay đổi logic kiểm tra và thêm người thân ---
            string cccd = request.PatientInfo?.CCCD?.Trim();
            string phone = request.PatientInfo?.Phone?.Trim();
            // Parse ngày sinh từ string với nhiều format
            DateTime dob = DateTime.MinValue;
            string dobStr = null;
            bool dobParseSuccess = false;
            
            // Debug logging
            Console.WriteLine($"=== DEBUG DOB PARSING ===");
            Console.WriteLine($"PatientInfo.Dob type: {(request.PatientInfo?.Dob != null ? request.PatientInfo.Dob.GetType().Name : "null")}");
            Console.WriteLine($"PatientInfo.Dob value: {request.PatientInfo?.Dob}");
            
            if (request.PatientInfo != null)
            {
                object dobObj = request.PatientInfo.Dob;
                Console.WriteLine($"dobObj type: {dobObj?.GetType().Name}");
                Console.WriteLine($"dobObj value: {dobObj}");
                
                if (dobObj is DateTime dt)
                {
                    dobStr = dt.ToString("yyyy-MM-dd");
                    Console.WriteLine($"Converted DateTime to string: {dobStr}");
                }
                else if (dobObj is string dobString)
                {
                    dobStr = dobString;
                    Console.WriteLine($"Using string directly: {dobStr}");
                }
                else
                {
                    dobStr = null;
                    Console.WriteLine($"dobObj is null or unknown type");
                }
                
                if (!string.IsNullOrEmpty(dobStr))
                {
                    string[] formats = { "yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" };
                    Console.WriteLine($"Trying to parse: '{dobStr}' with formats: {string.Join(", ", formats)}");
                    dobParseSuccess = DateTime.TryParseExact(dobStr, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dob);
                    Console.WriteLine($"Parse success: {dobParseSuccess}, Result: {dob:yyyy-MM-dd}");
                }
                else
                {
                    Console.WriteLine("dobStr is null or empty");
                }
            }
            else
            {
                Console.WriteLine("PatientInfo is null");
            }
            
            if (!dobParseSuccess || dob == DateTime.MinValue)
            {
                Console.WriteLine($"=== DOB PARSING FAILED ===");
                Console.WriteLine($"dobParseSuccess: {dobParseSuccess}");
                Console.WriteLine($"dob: {dob:yyyy-MM-dd}");
                Console.WriteLine($"dob == DateTime.MinValue: {dob == DateTime.MinValue}");
                return BadRequest(new { success = false, message = "Ngày sinh không hợp lệ! Vui lòng nhập đúng định dạng yyyy-MM-dd hoặc dd/MM/yyyy." });
            }
            
            Console.WriteLine($"=== DOB PARSING SUCCESS ===");
            Console.WriteLine($"Final dob: {dob:yyyy-MM-dd}");
            var existingPatient = await _context.Patients.FirstOrDefaultAsync(p => p.CCCD == cccd || p.Phone == phone);
            if (existingPatient != null)
            {
                await UpdatePatientInfoIfChanged(existingPatient, request.PatientInfo);
                // Sau khi cập nhật người thân, luôn lấy lại patient chính chủ
                patient = mainPatient;
            }
            else
            {
                var newPatient = new Patient
                {
                    Name = request.PatientInfo.Name,
                    Phone = phone,
                    Gender = !string.IsNullOrEmpty(request.PatientInfo.Gender) && Enum.TryParse<Gender>(request.PatientInfo.Gender, out var genderValue) ? genderValue : Gender.Male,
                    Dob = dob,
                    CCCD = cccd,
                    Address = string.IsNullOrEmpty(request.PatientInfo.Address) ? string.Empty : request.PatientInfo.Address,
                    InsuranceNumber = string.IsNullOrEmpty(request.PatientInfo.InsuranceNumber) ? string.Empty : request.PatientInfo.InsuranceNumber,
                    Allergies = string.IsNullOrEmpty(request.PatientInfo.Allergies) ? string.Empty : request.PatientInfo.Allergies,
                    BloodType = string.IsNullOrEmpty(request.PatientInfo.BloodType) ? string.Empty : request.PatientInfo.BloodType,
                    ImageURL = string.IsNullOrEmpty(request.PatientInfo.ImageURL) ? string.Empty : request.PatientInfo.ImageURL,
                    Status = PatientStatus.Active,
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreateBy = patient?.Name ?? "User",
                    UpdateBy = patient?.Name ?? "User",
                    Code = $"PAT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
                };
                _context.Patients.Add(newPatient);
                await _context.SaveChangesAsync();
                // Sau khi thêm người thân, luôn lấy lại patient chính chủ
                patient = mainPatient;
            }

            // Kiểm tra clinic có tồn tại và hoạt động
            var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == request.ClinicId && c.Status == ClinicStatus.Available);
            if (clinic == null)
                return BadRequest(new { success = false, message = "Phòng khám không tồn tại hoặc không hoạt động!" });

            // Kiểm tra doctor có tồn tại và làm việc tại clinic
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId && d.ClinicId == request.ClinicId);
            if (doctor == null)
                return BadRequest(new { success = false, message = "Bác sĩ không làm việc tại phòng khám này!" });

            // Kiểm tra service có tồn tại
            var service = await _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>().FirstOrDefaultAsync(s => s.Id == request.ServiceId);
            if (service == null)
                return BadRequest(new { success = false, message = "Dịch vụ không tồn tại!" });

            // Xác định thời gian bắt đầu/kết thúc theo ca
            TimeSpan start, end;
            if (request.Shift.ToLower() == "morning")
            {
                start = new TimeSpan(7, 30, 0);
                end = new TimeSpan(12, 0, 0);
            }
            else if (request.Shift.ToLower() == "afternoon")
            {
                start = new TimeSpan(13, 0, 0);
                end = new TimeSpan(17, 30, 0);
            }
            else
            {
                return BadRequest(new { success = false, message = "Ca làm việc không hợp lệ! Chỉ chấp nhận 'morning' hoặc 'afternoon'" });
            }

            // Kiểm tra bác sĩ có làm việc trong ca này không
            var doctorShift = await _context.Doctor_Shifts.FirstOrDefaultAsync(s => 
                s.DoctorId == request.DoctorId && 
                s.ShiftDate.Date == request.AppointmentDate.Date && 
                s.ShiftType.ToLower() == request.Shift.ToLower() &&
                s.StartTime <= start &&
                s.EndTime > start
            );
            if (doctorShift == null)
                return BadRequest(new { success = false, message = $"Bác sĩ không làm việc trong ca {request.Shift} vào ngày này!" });

            // Kiểm tra số lượng appointment trong ca
            int appointmentCount = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == request.AppointmentDate.Date
                    && a.StartTime == start
                    && (a.EndTime.HasValue && a.EndTime.Value == end)
                    && a.Doctor_Appointments.Any(da => da.DoctorId == request.DoctorId))
                .CountAsync();
            if (appointmentCount >= 5)
                return BadRequest(new { success = false, message = "Ca làm việc này đã đầy! Vui lòng chọn ca khác." });

            // Kiểm tra trùng lịch với StartTime cụ thể
            var existingAppointment = await _context.Appointments
                .Where(a => a.ClinicId == request.ClinicId
                    && a.AppointmentDate == request.AppointmentDate.Date
                    && a.StartTime == start)
                .Join(_context.Doctor_Appointments,
                    a => a.Id,
                    da => da.AppointmentId,
                    (a, da) => new { a, da })
                .FirstOrDefaultAsync(x => x.da.DoctorId == request.DoctorId);
            if (existingAppointment != null)
                return BadRequest(new { success = false, message = "Khung giờ này đã có người đặt! Vui lòng chọn giờ khác." });

            // Tạo mã lịch hẹn tự động
            string appointmentCode = await GenerateAppointmentCode();

            // 1. Tạo invoice trước
            var invoice = new SWP391_SE1914_ManageHospital.Models.Entities.Invoice
            {
                AppointmentId = 0, // tạm thời, sẽ cập nhật sau
                PatientId = patient.Id,
                InsuranceId = null, // Không gán mặc định 1, để null
                InitialAmount = 0,
                DiscountAmount = 0,
                TotalAmount = 0,
                Notes = "",
                Status = SWP391_SE1914_ManageHospital.Ultility.Status.InvoiceStatus.Unpaid,
                Name = $"Invoice - {patient.Name}",
                Code = $"INV-{Guid.NewGuid()}",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = patient.Name ?? "Patient",
                UpdateBy = patient.Name ?? "Patient"
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // 2. Tạo appointment với Id = invoice.Id
            var appointment = new SWP391_SE1914_ManageHospital.Models.Entities.Appointment
            {
                Id = invoice.Id,
                Name = $"Lịch hẹn - {patient.Name}",
                Code = appointmentCode,
                AppointmentDate = request.AppointmentDate.Date,
                StartTime = null,
                EndTime = null,
                Shift = request.Shift,
                Status = SWP391_SE1914_ManageHospital.Ultility.Status.AppointmentStatus.Scheduled,
                Note = !string.IsNullOrEmpty(request.Note) ? request.Note : $"Dịch vụ: {service.Name}",
                isSend = false,
                PatientId = patient.Id,
                ClinicId = request.ClinicId,
                ReceptionId = null,
                ServiceId = request.ServiceId,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = patient.Name ?? "Patient",
                UpdateBy = patient.Name ?? "Patient"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // 3. Tạo medical_record với AppointmentId = appointment.Id (không gán Id = appointment.Id)
            /*
            var medicalRecord = new SWP391_SE1914_ManageHospital.Models.Entities.Medical_Record
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = request.DoctorId,
                PrescriptionId = null, // Cho phép null
                DiseaseId = null, // Cho phép null
                Status = SWP391_SE1914_ManageHospital.Ultility.Status.MedicalRecordStatus.Open,
                Diagnosis = "",
                TestResults = "",
                Notes = "",
                Name = $"MedicalRecord - {appointment.Name}",
                Code = $"MR-{appointment.Code}",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = appointment.CreateBy,
                UpdateBy = appointment.UpdateBy
            };
            _context.Medical_Records.Add(medicalRecord);
            await _context.SaveChangesAsync();
            */

            // Cập nhật lại AppointmentId cho invoice nếu cần
            invoice.AppointmentId = appointment.Id;
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            // Liên kết với doctor
            var doctorAppointment = new SWP391_SE1914_ManageHospital.Models.Entities.Doctor_Appointment
            {
                DoctorId = request.DoctorId,
                AppointmentId = appointment.Id,
                Status = SWP391_SE1914_ManageHospital.Ultility.Status.DoctorAppointmentStatus.Assigned
            };
            _context.Doctor_Appointments.Add(doctorAppointment);
            await _context.SaveChangesAsync();

            // Trả về thông tin lịch hẹn đã tạo (fix triệt để null và lỗi format)
            var responseData = new
            {
                appointmentId = appointment?.Id ?? 0,
                appointmentCode = appointment?.Code ?? string.Empty,
                appointmentDate = appointment?.AppointmentDate != null && appointment.AppointmentDate != DateTime.MinValue
                    ? appointment.AppointmentDate.ToString("yyyy-MM-dd") : string.Empty,
                startTime = appointment.StartTime.HasValue ? SafeTimeSpanToString(appointment.StartTime.Value) : string.Empty,
                endTime = appointment?.EndTime.HasValue == true ? SafeTimeSpanToString(appointment.EndTime.Value) : null,
                shift = appointment?.Shift ?? "unknown",
                clinic = clinic != null ? new { id = clinic.Id, name = clinic.Name ?? "", address = clinic.Address ?? "" } : new { id = 0, name = "", address = "" },
                doctor = doctor != null ? new { id = doctor.Id, name = doctor.Name ?? "" } : new { id = 0, name = "" },
                service = service != null ? new { id = service.Id, name = service.Name ?? "" } : new { id = 0, name = "" },
                patient = patient != null ? new { id = patient.Id, name = patient.Name ?? "", phone = patient.Phone ?? "" } : new { id = 0, name = "", phone = "" },
                status = appointment?.Status.ToString() ?? string.Empty,
                note = appointment?.Note ?? string.Empty
            };

            return Ok(new 
            { 
                success = true, 
                message = "Đặt lịch hẹn thành công! Vui lòng kiểm tra email để xác nhận.", 
                data = responseData 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                success = false, 
                message = "Đã xảy ra lỗi trong quá trình đặt lịch. Vui lòng thử lại sau!", 
                error = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật thông tin bệnh nhân nếu có thay đổi
    /// </summary>
    private async Task UpdatePatientInfoIfChanged(Patient patient, SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment.PatientInfoDto? info)
    {
        if (info == null) return;

        bool isChanged = false;
        
        if (!string.IsNullOrEmpty(info.Name) && patient.Name != info.Name) 
        { 
            patient.Name = info.Name; 
            isChanged = true; 
        }
        
        if (!string.IsNullOrEmpty(info.Phone) && patient.Phone != info.Phone) 
        { 
            patient.Phone = info.Phone; 
            isChanged = true; 
        }
        
        if (!string.IsNullOrEmpty(info.Gender) && patient.Gender.ToString() != info.Gender) 
        { 
            if (Enum.TryParse(info.Gender, out Gender g)) 
            { 
                patient.Gender = g; 
                isChanged = true; 
            } 
        }
        
        if (info.Dob != default && patient.Dob != info.Dob) 
        { 
            patient.Dob = info.Dob; 
            isChanged = true; 
        }
        
        if (!string.IsNullOrEmpty(info.CCCD) && patient.CCCD != info.CCCD) 
        { 
            patient.CCCD = info.CCCD; 
            isChanged = true; 
        }
        
        if (!string.IsNullOrEmpty(info.Address) && patient.Address != info.Address) 
        { 
            patient.Address = info.Address; 
            isChanged = true; 
        }
        
        if (!string.IsNullOrEmpty(info.InsuranceNumber) && patient.InsuranceNumber != info.InsuranceNumber) 
        { 
            patient.InsuranceNumber = info.InsuranceNumber; 
            isChanged = true; 
        }
        
        if (!string.IsNullOrEmpty(info.Allergies) && patient.Allergies != info.Allergies) 
        { 
            patient.Allergies = info.Allergies; 
            isChanged = true; 
        }
        
        if (!string.IsNullOrEmpty(info.BloodType) && patient.BloodType != info.BloodType) 
        { 
            patient.BloodType = info.BloodType; 
            isChanged = true; 
        }

        if (isChanged) 
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Tạo mã lịch hẹn tự động
    /// </summary>
    private async Task<string> GenerateAppointmentCode()
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        var lastAppointment = await _context.Appointments
            .Where(a => a.Code.StartsWith($"APT{today}"))
            .OrderByDescending(a => a.Code)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastAppointment != null)
        {
            var lastNumberStr = lastAppointment.Code.Substring(11); // Bỏ "APT" + 8 số ngày
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"APT{today}{nextNumber:D3}"; // APT20241201001
    }

    [HttpGet("booked-times")]
    public async Task<IActionResult> GetBookedTimes([FromQuery] int clinicId, [FromQuery] int doctorId, [FromQuery] int serviceId, [FromQuery] DateTime date)
    {
        var booked = await _context.Appointments
            .Where(a => a.ClinicId == clinicId
                && a.AppointmentDate == date.Date)
            .Join(_context.Doctor_Appointments,
                a => a.Id,
                da => da.AppointmentId,
                (a, da) => new { a, da })
            .Where(x => x.da.DoctorId == doctorId)
            .Select(x => x.a.StartTime.HasValue ? x.a.StartTime.Value.ToString(@"hh\:mm") : "")
            .ToListAsync();
        return Ok(new { success = true, data = booked });
    }

    /// <summary>
    /// Test endpoint tạo appointment không cần authorize - CHỈ DÙNG CHO DEVELOPMENT
    /// </summary>
    [HttpPost("create-test")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAppointmentTest([FromBody] SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment.AppointmentCreateRequest request, [FromQuery] int? userId = null)
    {
        try
        {
            // Validation input
            if (request == null)
                return BadRequest(new { success = false, message = "Dữ liệu yêu cầu không hợp lệ!" });

            if (request.AppointmentDate.Date < DateTime.Now.Date)
                return BadRequest(new { success = false, message = "Không thể đặt lịch cho ngày trong quá khứ!" });

            // Kiểm tra Shift có hợp lệ không
            if (string.IsNullOrEmpty(request.Shift) || (request.Shift.ToLower() != "morning" && request.Shift.ToLower() != "afternoon"))
                return BadRequest(new { success = false, message = "Ca làm việc không hợp lệ! Chỉ chấp nhận 'morning' hoặc 'afternoon'" });

            // Lấy patient theo userId nếu có, không thì lấy patient đầu tiên
            Patient patient;
            if (userId.HasValue)
            {
                patient = await _context.Patients
                    .Where(p => p.UserId == userId.Value)
                    .OrderBy(p => p.Id)
                    .FirstOrDefaultAsync();
                if (patient == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy bệnh nhân nào với userId này!" });
            }
            else
            {
                patient = await _context.Patients.FirstOrDefaultAsync();
                if (patient == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy bệnh nhân nào trong hệ thống để test!" });
            }

            // Thêm logic tạo mới bệnh nhân cho người thân (giống API chính)
            string cccd = request.PatientInfo?.CCCD?.Trim();
            string phone = request.PatientInfo?.Phone?.Trim();
            // Parse ngày sinh từ string với nhiều format
            DateTime dob = DateTime.MinValue;
            string dobStr = null;
            bool dobParseSuccess = false;
            if (request.PatientInfo != null)
            {
                object dobObj = request.PatientInfo.Dob;
                if (dobObj is DateTime dt)
                {
                    dobStr = dt.ToString("yyyy-MM-dd");
                }
                else if (dobObj is string dobString)
                {
                    dobStr = dobString;
                }
                else
                {
                    dobStr = null;
                }
                if (!string.IsNullOrEmpty(dobStr))
                {
                    string[] formats = { "yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" };
                    dobParseSuccess = DateTime.TryParseExact(dobStr, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dob);
                }
            }
            if (!dobParseSuccess || dob == DateTime.MinValue)
            {
                return BadRequest(new { success = false, message = "Ngày sinh không hợp lệ! Vui lòng nhập đúng định dạng yyyy-MM-dd hoặc dd/MM/yyyy." });
            }
            var existingPatient = await _context.Patients.FirstOrDefaultAsync(p => p.CCCD == cccd || p.Phone == phone);
            if (existingPatient != null)
            {
                // Fix dữ liệu tạm thời nếu có trường nào bị null
                existingPatient.Address ??= string.Empty;
                existingPatient.InsuranceNumber ??= null;
                existingPatient.Allergies ??= null;
                existingPatient.BloodType ??= null;
                existingPatient.ImageURL ??= null;
                await _context.SaveChangesAsync();

                // Nếu thông tin bệnh nhân giống hoàn toàn thì không cập nhật lại, chỉ dùng lại bản ghi
                bool isSame = true;
                if (existingPatient.Name != request.PatientInfo.Name) isSame = false;
                if (existingPatient.Phone != phone) isSame = false;
                if (existingPatient.Gender.ToString() != request.PatientInfo.Gender) isSame = false;
                if (existingPatient.Dob != dob) isSame = false;
                if (existingPatient.CCCD != cccd) isSame = false;
                if (existingPatient.Address != (request.PatientInfo.Address ?? string.Empty)) isSame = false;
                if (existingPatient.InsuranceNumber != (request.PatientInfo.InsuranceNumber ?? null)) isSame = false;
                if (existingPatient.Allergies != (request.PatientInfo.Allergies ?? null)) isSame = false;
                if (existingPatient.BloodType != (request.PatientInfo.BloodType ?? null)) isSame = false;
                if (existingPatient.ImageURL != (request.PatientInfo.ImageURL ?? null)) isSame = false;

                if (!isSame)
                {
                    await UpdatePatientInfoIfChanged(existingPatient, request.PatientInfo);
                }
                // Sau khi cập nhật người thân, luôn lấy lại patient chính chủ
                patient = await _context.Patients
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.Id)
                    .FirstOrDefaultAsync();
            }
            else
            {
                var newPatient = new Patient
                {
                    Name = request.PatientInfo.Name,
                    Phone = phone,
                    Gender = !string.IsNullOrEmpty(request.PatientInfo.Gender) && Enum.TryParse<Gender>(request.PatientInfo.Gender, out var genderValue) ? genderValue : Gender.Male,
                    Dob = dob,
                    CCCD = cccd,
                    Address = string.IsNullOrEmpty(request.PatientInfo.Address) ? string.Empty : request.PatientInfo.Address,
                    InsuranceNumber = string.IsNullOrEmpty(request.PatientInfo.InsuranceNumber) ? string.Empty : request.PatientInfo.InsuranceNumber,
                    Allergies = string.IsNullOrEmpty(request.PatientInfo.Allergies) ? string.Empty : request.PatientInfo.Allergies,
                    BloodType = string.IsNullOrEmpty(request.PatientInfo.BloodType) ? string.Empty : request.PatientInfo.BloodType,
                    ImageURL = string.IsNullOrEmpty(request.PatientInfo.ImageURL) ? string.Empty : request.PatientInfo.ImageURL,
                    Status = PatientStatus.Active,
                    UserId = userId ?? 0,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreateBy = patient?.Name ?? "User",
                    UpdateBy = patient?.Name ?? "User",
                    Code = $"PAT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
                };
                _context.Patients.Add(newPatient);
                await _context.SaveChangesAsync();
                // Sau khi thêm người thân, luôn lấy lại patient chính chủ
                patient = await _context.Patients
                    .Where(p => p.UserId == (userId ?? 0))
                    .OrderBy(p => p.Id)
                    .FirstOrDefaultAsync();
            }

            // Kiểm tra clinic có tồn tại và hoạt động
            var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == request.ClinicId && c.Status == ClinicStatus.Available);
            if (clinic == null)
                return BadRequest(new { success = false, message = "Phòng khám không tồn tại hoặc không hoạt động!" });

            // Kiểm tra doctor có tồn tại và làm việc tại clinic
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == request.DoctorId && d.ClinicId == request.ClinicId);
            if (doctor == null)
                return BadRequest(new { success = false, message = "Bác sĩ không làm việc tại phòng khám này!" });

            // Kiểm tra service có tồn tại
            var service = await _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>().FirstOrDefaultAsync(s => s.Id == request.ServiceId);
            if (service == null)
                return BadRequest(new { success = false, message = "Dịch vụ không tồn tại!" });

            // Xác định thời gian bắt đầu/kết thúc theo ca
            TimeSpan start, end;
            if (request.Shift.ToLower() == "morning")
            {
                start = new TimeSpan(7, 30, 0);
                end = new TimeSpan(12, 0, 0);
            }
            else if (request.Shift.ToLower() == "afternoon")
            {
                start = new TimeSpan(13, 0, 0);
                end = new TimeSpan(17, 30, 0);
            }
            else
            {
                return BadRequest(new { success = false, message = "Ca làm việc không hợp lệ! Chỉ chấp nhận 'morning' hoặc 'afternoon'" });
            }

            // Kiểm tra bác sĩ có làm việc trong ca này không
            int dayOfWeek = (int)request.AppointmentDate.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // Sunday

            var doctorShift = await _context.Doctor_Shifts.FirstOrDefaultAsync(s => 
                s.DoctorId == request.DoctorId && 
                s.ShiftDate.Date == request.AppointmentDate.Date && 
                s.ShiftType.ToLower() == request.Shift.ToLower() &&
                s.StartTime <= start &&
                s.EndTime > start
            );

            if (doctorShift == null)
                return BadRequest(new { success = false, message = $"Bác sĩ không làm việc trong ca {request.Shift} vào ngày này!" });

            // Kiểm tra số lượng appointment trong ca
            int appointmentCount = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == request.AppointmentDate.Date
                    && a.StartTime == start
                    && (a.EndTime.HasValue && a.EndTime.Value == end)
                    && a.Doctor_Appointments.Any(da => da.DoctorId == request.DoctorId))
                .CountAsync();

            if (appointmentCount >= 5)
                return BadRequest(new { success = false, message = "Ca làm việc này đã đầy! Vui lòng chọn ca khác." });

            // Kiểm tra trùng lịch với StartTime cụ thể  
            var existingAppointment = await _context.Appointments
                .Where(a => a.ClinicId == request.ClinicId
                    && a.AppointmentDate == request.AppointmentDate.Date
                    && a.StartTime == start)
                .Join(_context.Doctor_Appointments,
                    a => a.Id,
                    da => da.AppointmentId,
                    (a, da) => new { a, da })
                .FirstOrDefaultAsync(x => x.da.DoctorId == request.DoctorId);

            if (existingAppointment != null)
                return BadRequest(new { success = false, message = "Khung giờ này đã có người đặt! Vui lòng chọn giờ khác." });

            // Tạo mã lịch hẹn tự động
            string appointmentCode = await GenerateAppointmentCode();

            // 1. Tạo invoice trước
            var invoice = new SWP391_SE1914_ManageHospital.Models.Entities.Invoice
            {
                AppointmentId = 0, // tạm thời, sẽ cập nhật sau
                PatientId = patient.Id,
                InsuranceId = null, // Không gán mặc định 1, để null
                InitialAmount = 0,
                DiscountAmount = 0,
                TotalAmount = 0,
                Notes = "",
                Status = SWP391_SE1914_ManageHospital.Ultility.Status.InvoiceStatus.Unpaid,
                Name = $"Invoice - {patient.Name}",
                Code = $"INV-{Guid.NewGuid()}",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "TEST_USER",
                UpdateBy = "TEST_USER"
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // 2. Tạo appointment với Id = invoice.Id
            var appointment = new SWP391_SE1914_ManageHospital.Models.Entities.Appointment
            {
                Id = invoice.Id,
                Name = $"Lịch hẹn Test - {patient.Name}",
                Code = appointmentCode,
                AppointmentDate = request.AppointmentDate.Date,
                StartTime = null,
                EndTime = null,
                Shift = request.Shift,
                Status = SWP391_SE1914_ManageHospital.Ultility.Status.AppointmentStatus.Scheduled,
                Note = !string.IsNullOrEmpty(request.Note) ? request.Note : $"Dịch vụ: {service.Name}",
                isSend = false,
                PatientId = patient.Id,
                ClinicId = request.ClinicId,
                ReceptionId = 1, // Mặc định
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "TEST_USER",
                UpdateBy = "TEST_USER"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Cập nhật lại AppointmentId cho invoice nếu cần
            invoice.AppointmentId = appointment.Id;
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            // Liên kết với doctor
            var doctorAppointment = new SWP391_SE1914_ManageHospital.Models.Entities.Doctor_Appointment
            {
                DoctorId = request.DoctorId,
                AppointmentId = appointment.Id,
                Status = SWP391_SE1914_ManageHospital.Ultility.Status.DoctorAppointmentStatus.Assigned
            };
            _context.Doctor_Appointments.Add(doctorAppointment);
            await _context.SaveChangesAsync();

            // Trả về thông tin lịch hẹn đã tạo (fix triệt để null và lỗi format)
            var responseData = new
            {
                appointmentId = appointment?.Id ?? 0,
                appointmentCode = appointment?.Code ?? string.Empty,
                appointmentDate = appointment?.AppointmentDate != null && appointment.AppointmentDate != DateTime.MinValue
                    ? appointment.AppointmentDate.ToString("yyyy-MM-dd") : string.Empty,
                startTime = appointment.StartTime.HasValue ? SafeTimeSpanToString(appointment.StartTime.Value) : string.Empty,
                endTime = appointment?.EndTime.HasValue == true ? SafeTimeSpanToString(appointment.EndTime.Value) : null,
                shift = appointment?.Shift ?? "unknown",
                clinic = clinic != null ? new { id = clinic.Id, name = clinic.Name ?? "", address = clinic.Address ?? "" } : new { id = 0, name = "", address = "" },
                doctor = doctor != null ? new { id = doctor.Id, name = doctor.Name ?? "" } : new { id = 0, name = "" },
                service = service != null ? new { id = service.Id, name = service.Name ?? "" } : new { id = 0, name = "" },
                patient = patient != null ? new { id = patient.Id, name = patient.Name ?? "", phone = patient.Phone ?? "" } : new { id = 0, name = "", phone = "" },
                status = appointment?.Status.ToString() ?? string.Empty,
                note = appointment?.Note ?? string.Empty,
                testMode = true
            };

            return Ok(new 
            { 
                success = true, 
                message = "[TEST MODE] Đặt lịch hẹn thành công!", 
                data = responseData 
            });
        }
        catch (Exception ex)
        {
            // Log lỗi chi tiết ra console
            Console.WriteLine(ex.ToString());
            if (ex.InnerException != null)
                Console.WriteLine("Inner: " + ex.InnerException.ToString());

            return StatusCode(500, new 
            { 
                success = false, 
                message = "Đã xảy ra lỗi trong quá trình đặt lịch. Vui lòng thử lại sau!", 
                error = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
    }

    [HttpPost("create-sample-shifts/{doctorId}")]
    public async Task<IActionResult> CreateSampleShifts(int doctorId, [FromQuery] int year, [FromQuery] int month)
    {
        try
        {
            // Kiểm tra xem doctor có tồn tại không
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy bác sĩ" });
            }

            // Xóa các shifts cũ trong tháng nếu có
            var existingShifts = await _context.Doctor_Shifts
                .Where(s => s.DoctorId == doctorId && 
                           s.ShiftDate.Year == year && 
                           s.ShiftDate.Month == month)
                .ToListAsync();
            
            if (existingShifts.Any())
            {
                _context.Doctor_Shifts.RemoveRange(existingShifts);
                await _context.SaveChangesAsync();
            }

            // Tạo lịch làm việc mẫu: Thứ 2-6, ca sáng và chiều
            var shifts = new List<Doctor_Shift>();
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            for (var date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                // Chỉ tạo lịch cho thứ 2-6 (Monday = 1, Friday = 5)
                var dayOfWeek = (int)date.DayOfWeek;
                if (dayOfWeek >= 1 && dayOfWeek <= 5) // Monday to Friday
                {
                    // Ca sáng: 8:00 - 12:00
                    shifts.Add(new Doctor_Shift
                    {
                        DoctorId = doctorId,
                        ShiftDate = date,
                        ShiftType = "morning",
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0),
                        Notes = "Ca sáng",
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        CreateBy = "System",
                        UpdateBy = "System"
                    });

                    // Ca chiều: 13:00 - 17:00
                    shifts.Add(new Doctor_Shift
                    {
                        DoctorId = doctorId,
                        ShiftDate = date,
                        ShiftType = "afternoon",
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        Notes = "Ca chiều",
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        CreateBy = "System",
                        UpdateBy = "System"
                    });
                }
            }

            _context.Doctor_Shifts.AddRange(shifts);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Đã tạo {shifts.Count} ca làm việc cho bác sĩ {doctor.Name} trong tháng {month}/{year}",
                data = new
                {
                    doctorId = doctorId,
                    doctorName = doctor.Name,
                    year = year,
                    month = month,
                    shiftsCount = shifts.Count,
                    workingDays = shifts.Select(s => s.ShiftDate.Date).Distinct().Count()
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Lỗi khi tạo lịch làm việc mẫu",
                error = ex.Message
            });
        }
    }

    [HttpDelete("clear-sample-data")]
    public async Task<IActionResult> ClearSampleData()
    {
        try
        {
            // Xóa TẤT CẢ dữ liệu mẫu trong bảng doctor_shifts
            var allSampleShifts = await _context.Doctor_Shifts
                .Where(s => s.CreateBy == "System" || s.Notes.Contains("Ca sáng") || s.Notes.Contains("Ca chiều"))
                .ToListAsync();

            if (allSampleShifts.Any())
            {
                _context.Doctor_Shifts.RemoveRange(allSampleShifts);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Đã xóa {allSampleShifts.Count} ca làm việc mẫu khỏi database",
                    data = new
                    {
                        deletedCount = allSampleShifts.Count,
                        shifts = allSampleShifts.Select(s => new
                        {
                            doctorId = s.DoctorId,
                            shiftDate = s.ShiftDate.ToString("yyyy-MM-dd"),
                            shiftType = s.ShiftType,
                            notes = s.Notes
                        }).ToList()
                    }
                });
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    message = "Không có dữ liệu mẫu nào để xóa",
                    data = new { deletedCount = 0 }
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Lỗi khi xóa dữ liệu mẫu",
                error = ex.Message
            });
        }
    }

    [HttpGet("doctor-working-days/{doctorId}")]
    public async Task<IActionResult> GetDoctorWorkingDays(int doctorId, [FromQuery] int year, [FromQuery] int month)
    {
        try
        {
            const int MAX_APPOINTMENTS_PER_DAY = 5;
            
            // Kiểm tra xem doctor có tồn tại không
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy bác sĩ" });
            }
            
            // Lấy tất cả shifts của doctor trong tháng (CHỈ lấy từ database, KHÔNG tự tạo)
            var doctorShifts = await _context.Doctor_Shifts
                .Where(s => s.DoctorId == doctorId && 
                           s.ShiftDate.Year == year && 
                           s.ShiftDate.Month == month)
                .ToListAsync();

            // Lấy tất cả shift request đã duyệt nghỉ của bác sĩ trong tháng
            var approvedShiftRequests = await _context.ShiftRequests
                .Where(r => r.DoctorId == doctorId && r.Status == "Approved")
                .Select(r => r.ShiftId)
                .ToListAsync();

            // Loại bỏ các ca trực đã được duyệt nghỉ
            var workingShifts = doctorShifts.Where(s => !approvedShiftRequests.Contains(s.Id)).ToList();

            // Nếu không có lịch làm việc, trả về danh sách rỗng
            if (!workingShifts.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "Bác sĩ không có lịch làm việc trong tháng này",
                    data = new List<object>()
                });
            }

            // Lấy các ngày mà doctor làm việc (sau khi loại ngày nghỉ)
            var workingDays = workingShifts.Select(s => s.ShiftDate.Date).Distinct().ToList();

            // Tạo danh sách các ngày trong tháng mà doctor làm việc
            var workingDaysResult = new List<object>();
            var today = DateTime.Today;

            foreach (var workingDate in workingDays)
            {
                // Kiểm tra nếu ngày này không phải ngày trong quá khứ
                if (workingDate >= today)
                {
                    // Đếm số lượng appointment đã đặt cho ngày này
                    var appointmentCount = await _context.Doctor_Appointments
                        .Where(da => da.DoctorId == doctorId &&
                                    da.Appointment.AppointmentDate.Date == workingDate.Date)
                        .CountAsync();

                    workingDaysResult.Add(new
                    {
                        day = workingDate.Day,
                        appointmentCount = appointmentCount,
                        isAvailable = appointmentCount < MAX_APPOINTMENTS_PER_DAY,
                        maxSlots = MAX_APPOINTMENTS_PER_DAY
                    });
                }
            }

            return Ok(new
            {
                success = true,
                message = "Lấy lịch làm việc của bác sĩ thành công",
                data = workingDaysResult
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message,
                data = (object)null
            });
        }
    }

    [HttpGet("available-time-slots")]
    public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] int doctorId, [FromQuery] string date)
    {
        try
        {
            const int MAX_SLOTS_PER_DAY = 10;
            var countableStatuses = new[] { "chưa tới", "đang khám", "đến muộn" };

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid date format. Expected format: yyyy-MM-dd",
                    data = (object)null
                });
            }

            var doctorShifts = await _context.Doctor_Shifts
                .Where(s => s.DoctorId == doctorId && s.ShiftDate.Date == parsedDate.Date)
                .ToListAsync();

            var approvedShiftRequests = await _context.ShiftRequests
                .Where(r => r.DoctorId == doctorId && r.Status == "Approved")
                .Select(r => r.ShiftId)
                .ToListAsync();
            doctorShifts = doctorShifts.Where(s => !approvedShiftRequests.Contains(s.Id)).ToList();

            // Ca sáng: 07:00 - 12:00
            var morningStart = new TimeSpan(7, 0, 0);
            var morningEnd = new TimeSpan(12, 0, 0);
            // Ca chiều: 13:00 - 17:00
            var afternoonStart = new TimeSpan(13, 0, 0);
            var afternoonEnd = new TimeSpan(17, 0, 0);

            // Đếm số lịch hẹn đã đặt trong ngày (chỉ đếm các trạng thái còn hiệu lực)
            var bookedSlotsCount = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == parsedDate.Date
                    && a.Doctor_Appointments.Any(da => da.DoctorId == doctorId)
                    && (a.Status == SWP391_SE1914_ManageHospital.Ultility.Status.AppointmentStatus.Scheduled
                        || a.Status == SWP391_SE1914_ManageHospital.Ultility.Status.AppointmentStatus.InProgress
                        || a.Status == SWP391_SE1914_ManageHospital.Ultility.Status.AppointmentStatus.Late))
                .CountAsync();

            // Kiểm tra ca sáng có available không
            bool morningAvailable = false;
            if (doctorShifts.Any(s => s.ShiftType.ToLower() == "morning"))
            {
                morningAvailable = bookedSlotsCount < MAX_SLOTS_PER_DAY;
            }

            // Kiểm tra ca chiều có available không  
            bool afternoonAvailable = false;
            if (doctorShifts.Any(s => s.ShiftType.ToLower() == "afternoon"))
            {
                afternoonAvailable = bookedSlotsCount < MAX_SLOTS_PER_DAY;
            }

            // Nếu không có ca nào làm việc cả ngày
            if (!morningAvailable && !afternoonAvailable)
            {
                return Ok(new
                {
                    success = true,
                    message = "Bác sĩ không làm việc vào ngày này",
                    data = new
                    {
                        morning = new { available = false, reason = "Bác sĩ không làm việc hoặc đã xin nghỉ ca sáng" },
                        afternoon = new { available = false, reason = "Bác sĩ không làm việc hoặc đã xin nghỉ ca chiều" }
                    }
                });
            }

            return Ok(new
            {
                success = true,
                message = "OK",
                data = new
                {
                    morning = new { available = morningAvailable, reason = "Bác sĩ không làm việc ca sáng" },
                    afternoon = new { available = afternoonAvailable, reason = "Bác sĩ không làm việc ca chiều" }
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message,
                data = (object)null
            });
        }
    }

    // Thêm hàm helper an toàn cho TimeSpan
    private string SafeTimeSpanToString(TimeSpan? time)
    {
        if (time == null) return string.Empty;
        try { return time.Value.ToString(@"hh\:mm"); } catch { return string.Empty; }
    }
    private string SafeTimeSpanToString(TimeSpan time)
    {
        try { return time.ToString(@"hh\:mm"); } catch { return string.Empty; }
    }

    #region Patient Appointment Management APIs

    /// <summary>
    /// Lấy danh sách lịch hẹn của bệnh nhân hiện tại
    /// </summary>
    /// <param name="status">Trạng thái lịch hẹn (all, today, completed, cancelled, scheduled, noshow)</param>
    /// <param name="searchTerm">Từ khóa tìm kiếm</param>
    /// <param name="page">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Danh sách lịch hẹn của bệnh nhân</returns>
    [Authorize]
    [HttpGet("patient-appointments")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetPatientAppointments(
        [FromQuery] int userId,
        [FromQuery] string status = "all",
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            // Lấy tất cả bệnh nhân (bao gồm người thân) của user
            var patients = await _context.Patients.Where(p => p.UserId == userId).ToListAsync();
            if (patients == null || patients.Count == 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "Không có lịch hẹn nào cho user này",
                    data = new {
                        appointments = Array.Empty<object>(),
                        pagination = new {
                            page = page,
                            pageSize = pageSize,
                            totalCount = 0,
                            totalPages = 0
                        }
                    }
                });
            }
            var patientIds = patients.Select(p => p.Id).ToList();

            // Xây dựng query cơ bản lấy lịch hẹn của tất cả bệnh nhân thuộc user
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Include(a => a.Service)
                .Include(a => a.Doctor_Appointments)
                    .ThenInclude(da => da.Doctor)
                .Where(a => patientIds.Contains(a.PatientId));

            // Áp dụng filter theo trạng thái
            switch (status.ToLower())
            {
                case "today":
                    query = query.Where(a => a.AppointmentDate.Date == DateTime.Now.Date);
                    break;
                case "completed":
                    query = query.Where(a => a.Status == AppointmentStatus.Completed);
                    break;
                case "cancelled":
                    query = query.Where(a => a.Status == AppointmentStatus.Cancelled);
                    break;
                case "scheduled":
                    query = query.Where(a => a.Status == AppointmentStatus.Scheduled);
                    break;
                case "late":
                    query = query.Where(a => a.Status == AppointmentStatus.Late);
                    break;
                case "all":
                default:
                    // Không filter gì cả
                    break;
            }

            // Áp dụng tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a =>
                    (a.Patient != null && a.Patient.Name.Contains(searchTerm)) ||
                    (a.Clinic != null && a.Clinic.Name.Contains(searchTerm)) ||
                    (a.Service != null && a.Service.Name.Contains(searchTerm)) ||
                    (a.Doctor_Appointments != null && a.Doctor_Appointments.Any(da => da.Doctor != null && da.Doctor.Name.Contains(searchTerm))) ||
                    (a.Note != null && a.Note.Contains(searchTerm))
                );
            }

            // Sắp xếp theo ngày gần nhất
            query = query.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.StartTime);

            // Tính tổng số bản ghi
            var totalCount = await query.CountAsync();

            // Phân trang
            var appointments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("dd/MM/yyyy"),
                    startTime = SafeTimeSpanToString(a.StartTime),
                    endTime = SafeTimeSpanToString(a.EndTime),
                    status = a.Status.ToString(),
                    statusText = GetStatusText(a.Status),
                    note = a.Note ?? string.Empty,
                    isSend = a.isSend,
                    patient = a.Patient != null ? new
                    {
                        id = a.Patient.Id,
                        name = a.Patient.Name ?? string.Empty,
                        phone = a.Patient.Phone ?? string.Empty,
                        gender = a.Patient.Gender != null ? a.Patient.Gender.ToString() : string.Empty
                    } : null,
                    clinic = a.Clinic != null ? new
                    {
                        id = a.Clinic.Id,
                        name = a.Clinic.Name ?? string.Empty,
                        address = a.Clinic.Address ?? string.Empty
                    } : null,
                    service = a.Service != null ? new
                    {
                        id = a.Service.Id,
                        name = a.Service.Name ?? string.Empty,
                        price = a.Service.Price != null ? a.Service.Price : 0
                    } : null,
                    doctors = a.Doctor_Appointments != null ? a.Doctor_Appointments
                        .Where(da => da.Doctor != null)
                        .Select(da => (object)new {
                            id = da.Doctor.Id,
                            name = da.Doctor.Name ?? string.Empty,
                            imageUrl = da.Doctor.ImageURL ?? string.Empty
                        }).ToList() : new List<object>(),
                    createDate = a.CreateDate.ToString("dd/MM/yyyy HH:mm"),
                    updateDate = a.UpdateDate.HasValue ? a.UpdateDate.Value.ToString() : null
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Lấy danh sách lịch hẹn thành công",
                data = new
                {
                    appointments = appointments,
                    pagination = new
                    {
                        page = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("API ERROR in GetPatientAppointments: " + ex.ToString());
            return StatusCode(500, new
            {
                success = false,
                message = "Đã xảy ra lỗi khi lấy danh sách lịch hẹn",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Lấy chi tiết lịch hẹn theo ID
    /// </summary>
    /// <param name="appointmentId">ID lịch hẹn</param>
    /// <returns>Chi tiết lịch hẹn</returns>
    [Authorize]
    [HttpGet("patient-appointments/{appointmentId}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetPatientAppointmentDetail(int appointmentId)
    {
        try
        {
            // Lấy user ID từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Không thể xác định người dùng",
                    data = (object)null
                });
            }

            // Lấy tất cả bệnh nhân (bao gồm người thân) của user
            var patients = await _context.Patients.Where(p => p.UserId == userId).ToListAsync();
            if (patients == null || patients.Count == 0)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin bệnh nhân",
                    data = (object)null
                });
            }
            var patientIds = patients.Select(p => p.Id).ToList();

            // Lấy lịch hẹn của bất kỳ bệnh nhân nào thuộc user
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && patientIds.Contains(a.PatientId));

            if (appointment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy lịch hẹn",
                    data = (object)null
                });
            }

            var result = new
            {
                id = appointment.Id,
                appointmentDate = appointment.AppointmentDate.ToString("dd/MM/yyyy"),
                startTime = SafeTimeSpanToString(appointment.StartTime),
                endTime = SafeTimeSpanToString(appointment.EndTime),
                status = appointment.Status.ToString(),
                statusText = GetStatusText(appointment.Status),
                note = appointment.Note,
                isSend = appointment.isSend,
                patient = new
                {
                    id = appointment.Patient.Id,
                    name = appointment.Patient.Name,
                    phone = appointment.Patient.Phone,
                    gender = appointment.Patient.Gender.ToString(),
                    dob = appointment.Patient.Dob.ToString("dd/MM/yyyy"),
                    address = appointment.Patient.Address,
                    cccd = appointment.Patient.CCCD,
                    insuranceNumber = appointment.Patient.InsuranceNumber,
                    allergies = appointment.Patient.Allergies,
                    bloodType = appointment.Patient.BloodType,
                    imageUrl = appointment.Patient.ImageURL
                },
                clinic = new
                {
                    id = appointment.Clinic.Id,
                    name = appointment.Clinic.Name,
                    address = string.IsNullOrWhiteSpace(appointment.Clinic.Address) ? "Chưa cập nhật" : appointment.Clinic.Address,
                    email = appointment.Clinic.Email
                },
                service = appointment.Service != null ? new
                {
                    id = appointment.Service.Id,
                    name = appointment.Service.Name,
                    price = appointment.Service.Price,
                    description = appointment.Service.Description
                } : null,
                doctors = appointment.Doctor_Appointments.Select(da => new
                {
                    id = da.Doctor.Id,
                    name = da.Doctor.Name,
                    imageUrl = da.Doctor.ImageURL,
                    phone = da.Doctor.Phone
                }).ToList(),
                invoice = appointment.Invoice != null ? new
                {
                    id = appointment.Invoice.Id,
                    totalAmount = appointment.Invoice.TotalAmount,
                    status = appointment.Invoice.Status.ToString()
                } : null,
                createDate = appointment.CreateDate.ToString("dd/MM/yyyy HH:mm"),
                updateDate = appointment.UpdateDate.HasValue ? appointment.UpdateDate.Value.ToString() : null
            };

            return Ok(new
            {
                success = true,
                message = "Lấy chi tiết lịch hẹn thành công",
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Đã xảy ra lỗi khi lấy chi tiết lịch hẹn",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật lịch hẹn của bệnh nhân
    /// </summary>
    /// <param name="appointmentId">ID lịch hẹn</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Kết quả cập nhật</returns>
    [Authorize]
    [HttpPut("patient-appointments/{appointmentId}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdatePatientAppointment(
        int appointmentId,
        [FromBody] AppointmentUpdateRequest request)
    {
        try
        {
            // Lấy user ID từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Không thể xác định người dùng",
                    data = (object)null
                });
            }

            // Lấy patient ID từ user ID (chỉ lấy bệnh nhân chính)
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin bệnh nhân",
                    data = (object)null
                });
            }

            // Lấy lịch hẹn
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == patient.Id);

            if (appointment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy lịch hẹn",
                    data = (object)null
                });
            }

            // Kiểm tra trạng thái lịch hẹn
            bool canEdit = false;
            if (appointment.Status == AppointmentStatus.Scheduled)
            {
                canEdit = true;
            }
            else if (appointment.Status == AppointmentStatus.Cancelled && (appointment.StartTime.HasValue ? DateTime.Now < appointment.AppointmentDate.Add(appointment.StartTime.Value) : DateTime.Now < appointment.AppointmentDate))
            {
                canEdit = true;
            }
            if (!canEdit)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Chỉ có thể cập nhật lịch hẹn đang chờ xác nhận hoặc đã hủy nhưng chưa đến thời gian hẹn",
                    data = (object)null
                });
            }

            // Cập nhật thông tin
            if (request.AppointmentDate.HasValue)
            {
                appointment.AppointmentDate = request.AppointmentDate.Value;
            }

            if (!string.IsNullOrEmpty(request.Shift))
            {
                appointment.Shift = request.Shift;
            }

            if (request.Note != null)
            {
                appointment.Note = request.Note;
            }

            appointment.UpdateDate = DateTime.Now;

            // Nếu có trường Status và là 'Scheduled' thì cho phép khôi phục trạng thái
            if (!string.IsNullOrEmpty(request.Status) && request.Status == "Scheduled")
            {
                appointment.Status = AppointmentStatus.Scheduled;
            }
            // Nếu có trường Status và là 'InProgress' thì cập nhật trạng thái và StartTime
            if (!string.IsNullOrEmpty(request.Status) && request.Status == "InProgress")
            {
                appointment.Status = AppointmentStatus.InProgress;
                if (!appointment.StartTime.HasValue)
                {
                    appointment.StartTime = DateTime.Now.TimeOfDay;
                }
                
                // Tự động tạo medical record khi bắt đầu khám
                await CreateMedicalRecordForAppointment(appointment);
            }
            // Nếu có trường Status và là 'Completed' thì cập nhật trạng thái và EndTime
            if (!string.IsNullOrEmpty(request.Status) && request.Status == "Completed")
            {
                appointment.Status = AppointmentStatus.Completed;
                if (!appointment.EndTime.HasValue)
                {
                    appointment.EndTime = DateTime.Now.TimeOfDay;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cập nhật lịch hẹn thành công",
                data = new
                {
                    id = appointment.Id,
                    appointmentDate = appointment.AppointmentDate.ToString("dd/MM/yyyy"),
                    startTime = SafeTimeSpanToString(appointment.StartTime),
                    note = appointment.Note,
                    updateDate = appointment.UpdateDate.HasValue ? appointment.UpdateDate.Value.ToString() : null
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Đã xảy ra lỗi khi cập nhật lịch hẹn",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Hủy lịch hẹn của bệnh nhân
    /// </summary>
    /// <param name="appointmentId">ID lịch hẹn</param>
    /// <param name="request">Lý do hủy</param>
    /// <returns>Kết quả hủy lịch hẹn</returns>
    [Authorize]
    [HttpPut("patient-appointments/{appointmentId}/cancel")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CancelPatientAppointment(
        int appointmentId,
        [FromBody] AppointmentCancelRequest request)
    {
        try
        {
            // Lấy user ID từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Không thể xác định người dùng",
                    data = (object)null
                });
            }

            // Lấy tất cả bệnh nhân (bao gồm người thân) của user
            var patients = await _context.Patients.Where(p => p.UserId == userId).ToListAsync();
            if (patients == null || patients.Count == 0)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin bệnh nhân",
                    data = (object)null
                });
            }
            var patientIds = patients.Select(p => p.Id).ToList();

            // Lấy lịch hẹn của bất kỳ bệnh nhân nào thuộc user
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && patientIds.Contains(a.PatientId));

            if (appointment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy lịch hẹn",
                    data = (object)null
                });
            }

            // Kiểm tra trạng thái lịch hẹn
            if (appointment.Status != AppointmentStatus.Scheduled)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Chỉ có thể hủy lịch hẹn đang chờ xác nhận",
                    data = (object)null
                });
            }

            // Cập nhật trạng thái
            appointment.Status = AppointmentStatus.Cancelled;
            appointment.Note = request.Reason ?? "Bệnh nhân hủy lịch hẹn";
            appointment.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Hủy lịch hẹn thành công",
                data = new
                {
                    id = appointment.Id,
                    status = appointment.Status.ToString(),
                    statusText = GetStatusText(appointment.Status),
                    note = appointment.Note,
                    updateDate = appointment.UpdateDate.HasValue ? appointment.UpdateDate.Value.ToString() : null
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Đã xảy ra lỗi khi hủy lịch hẹn",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Tìm kiếm lịch hẹn của bệnh nhân
    /// </summary>
    /// <param name="searchTerm">Từ khóa tìm kiếm</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <param name="status">Trạng thái</param>
    /// <param name="page">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Kết quả tìm kiếm</returns>
    [Authorize]
    [HttpGet("patient-appointments/search")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> SearchPatientAppointments(
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            // Lấy user ID từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Không thể xác định người dùng",
                    data = (object)null
                });
            }

            // Lấy tất cả bệnh nhân (bao gồm người thân) của user
            var patients = await _context.Patients.Where(p => p.UserId == userId).ToListAsync();
            if (patients == null || patients.Count == 0)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin bệnh nhân",
                    data = (object)null
                });
            }
            var patientIds = patients.Select(p => p.Id).ToList();

            // Xây dựng query cơ bản
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Include(a => a.Service)
                .Include(a => a.Doctor_Appointments)
                    .ThenInclude(da => da.Doctor)
                .Where(a => patientIds.Contains(a.PatientId));

            // Áp dụng filter theo ngày
            if (startDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date <= endDate.Value.Date);
            }

            // Áp dụng filter theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                switch (status.ToLower())
                {
                    case "completed":
                        query = query.Where(a => a.Status == AppointmentStatus.Completed);
                        break;
                    case "cancelled":
                        query = query.Where(a => a.Status == AppointmentStatus.Cancelled);
                        break;
                    case "scheduled":
                        query = query.Where(a => a.Status == AppointmentStatus.Scheduled);
                        break;
                    case "late":
                        query = query.Where(a => a.Status == AppointmentStatus.Late);
                        break;
                }
            }

            // Áp dụng tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a =>
                    a.Patient.Name.Contains(searchTerm) ||
                    a.Clinic.Name.Contains(searchTerm) ||
                    (a.Service != null && a.Service.Name.Contains(searchTerm)) ||
                    a.Doctor_Appointments.Any(da => da.Doctor.Name.Contains(searchTerm)) ||
                    (a.Note != null && a.Note.Contains(searchTerm))
                );
            }

            // Sắp xếp theo ngày gần nhất
            query = query.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.StartTime);

            // Tính tổng số bản ghi
            var totalCount = await query.CountAsync();

            // Phân trang
            var appointments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("dd/MM/yyyy"),
                    startTime = SafeTimeSpanToString(a.StartTime),
                    endTime = SafeTimeSpanToString(a.EndTime),
                    status = a.Status.ToString(),
                    statusText = GetStatusText(a.Status),
                    note = a.Note,
                    clinic = new
                    {
                        id = a.Clinic.Id,
                        name = a.Clinic.Name,
                        address = a.Clinic.Address
                    },
                    service = a.Service != null ? new
                    {
                        id = a.Service.Id,
                        name = a.Service.Name,
                        price = a.Service.Price
                    } : null,
                    doctors = a.Doctor_Appointments.Select(da => new
                    {
                        id = da.Doctor.Id,
                        name = da.Doctor.Name,
                        imageUrl = da.Doctor.ImageURL
                    }).ToList(),
                    createDate = a.CreateDate.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Tìm kiếm lịch hẹn thành công",
                data = new
                {
                    appointments = appointments,
                    pagination = new
                    {
                        page = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    },
                    filters = new
                    {
                        searchTerm = searchTerm,
                        startDate = startDate.HasValue ? startDate.Value.ToString("dd/MM/yyyy") : null,
                        endDate = endDate.HasValue ? endDate.Value.ToString("dd/MM/yyyy") : null,
                        status = status
                    }
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Đã xảy ra lỗi khi tìm kiếm lịch hẹn",
                error = ex.Message
            });
        }
    }

    #endregion

    // Helper method để lấy text trạng thái
    private string GetStatusText(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Scheduled => "Đã lên lịch",
            AppointmentStatus.Completed => "Đã hoàn thành",
            AppointmentStatus.Cancelled => "Đã hủy",
            AppointmentStatus.Late => "Đến muộn",
            _ => "Không xác định"
        };
    }

    /// <summary>
    /// API test lấy danh sách lịch hẹn theo userId, không cần xác thực
    /// </summary>
    [HttpGet("patient-appointments/test")]
    public async Task<IActionResult> GetPatientAppointmentsTest([FromQuery] int userId)
    {
        // Lấy tất cả bệnh nhân (bao gồm người thân) của user
        var patients = await _context.Patients.Where(p => p.UserId == userId).ToListAsync();
        if (patients == null || patients.Count == 0)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông tin bệnh nhân",
                data = (object)null
            });
        }
        var patientIds = patients.Select(p => p.Id).ToList();
        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Clinic)
            .Include(a => a.Service)
            .Include(a => a.Doctor_Appointments)
                .ThenInclude(da => da.Doctor)
            .Where(a => patientIds.Contains(a.PatientId));
        var appointments = await query.ToListAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách lịch hẹn thành công (test mode)",
            data = new
            {
                appointments = appointments.Select(a => new {
                    id = a.Id,
                    appointmentDate = a.AppointmentDate.ToString("dd/MM/yyyy"),
                    startTime = SafeTimeSpanToString(a.StartTime),
                    endTime = SafeTimeSpanToString(a.EndTime),
                    status = a.Status.ToString(),
                    note = a.Note,
                    patient = a.Patient != null ? new { id = a.Patient.Id, name = a.Patient.Name } : null,
                    clinic = a.Clinic != null ? new { id = a.Clinic.Id, name = a.Clinic.Name, address = string.IsNullOrWhiteSpace(a.Clinic.Address) ? "Chưa cập nhật" : a.Clinic.Address } : null,
                    service = a.Service != null ? new { id = a.Service.Id, name = a.Service.Name } : null,
                    doctors = a.Doctor_Appointments != null ? a.Doctor_Appointments.Where(da => da.Doctor != null).Select(da => (object)new { id = da.Doctor.Id, name = da.Doctor.Name }).ToList() : new List<object>(),
                    createDate = a.CreateDate.ToString("dd/MM/yyyy HH:mm")
                }).ToList()
            }
        });
    }

    /// <summary>
    /// Tự động tạo medical record khi appointment chuyển sang trạng thái InProgress
    /// </summary>
    /// <param name="appointment">Appointment cần tạo medical record</param>
    private async Task CreateMedicalRecordForAppointment(Appointment appointment)
    {
        try
        {
            // Kiểm tra xem đã có medical record cho appointment này chưa
            var existingMedicalRecord = await _context.Medical_Records
                .FirstOrDefaultAsync(mr => mr.AppointmentId == appointment.Id);
            
            if (existingMedicalRecord != null)
            {
                // Nếu đã có thì không tạo mới
                return;
            }

            // Lấy thông tin doctor từ Doctor_Appointment
            var doctorAppointment = await _context.Doctor_Appointments
                .Include(da => da.Doctor)
                .FirstOrDefaultAsync(da => da.AppointmentId == appointment.Id);
            
            if (doctorAppointment?.Doctor == null)
            {
                // Nếu không tìm thấy doctor thì không tạo medical record
                return;
            }

            // Tạo medical record mới
            var medicalRecord = new Medical_Record
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = doctorAppointment.DoctorId,
                PrescriptionId = null, // Sẽ được cập nhật sau khi có prescription
                DiseaseId = null, // Sẽ được cập nhật sau khi chẩn đoán
                Status = MedicalRecordStatus.Open,
                Diagnosis = "Chưa chẩn đoán", // Giá trị mặc định
                TestResults = "Chưa có kết quả xét nghiệm", // Giá trị mặc định
                Notes = $"Tự động tạo khi bắt đầu khám - {DateTime.Now:dd/MM/yyyy HH:mm}",
                Name = $"Hồ sơ khám - {appointment.Name}",
                Code = $"MR-{appointment.Code}",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = appointment.CreateBy,
                UpdateBy = appointment.UpdateBy
            };

            _context.Medical_Records.Add(medicalRecord);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log lỗi nhưng không làm gián đoạn quá trình cập nhật appointment
            Console.WriteLine($"Lỗi khi tạo medical record: {ex.Message}");
        }
    }
} 