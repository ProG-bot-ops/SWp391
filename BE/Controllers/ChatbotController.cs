using Microsoft.AspNetCore.Mvc;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ChatbotController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/chatbot/departments
        [HttpGet("departments")]
        public async Task<ActionResult<object>> GetDepartments()
        {
            try
            {
                var departments = await _context.Departments
                    .Where(d => d.Status == DepartmentStatus.Active)
                    .Select(d => new { d.Id, d.Name })
                    .ToListAsync();

                return Ok(new { departments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/chatbot/doctors
        [HttpGet("doctors")]
        public async Task<ActionResult<object>> GetDoctors([FromQuery] string department)
        {
            try
            {
                var doctors = await _context.Doctors
                    .Include(d => d.Department)
                    .Where(d => d.Status == DoctorStatus.Available && d.Department.Name == department)
                    .Select(d => new { 
                        d.Id, 
                        Name = d.User.Email, // Using email as name since User doesn't have Name field
                        Experience = d.YearOfExperience, 
                        Specialty = d.Department.Name 
                    })
                    .ToListAsync();

                return Ok(new { doctors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/chatbot/services
        [HttpGet("services")]
        public async Task<ActionResult<object>> GetServices([FromQuery] string doctor)
        {
            try
            {
                // Get doctor's department first
                var doctorEntity = await _context.Doctors
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.User.Email == doctor);

                if (doctorEntity == null)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                var services = await _context.Services
                    .Where(s => s.Status == ServiceStatus.Active && s.DepartmentId == doctorEntity.DepartmentId)
                    .Select(s => new { s.Id, s.Name, s.Price })
                    .ToListAsync();

                return Ok(new { services });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/chatbot/shifts
        [HttpGet("shifts")]
        public async Task<ActionResult<object>> GetShifts([FromQuery] string doctor, [FromQuery] string date)
        {
            try
            {
                // Parse date from DD/MM/YYYY format
                if (!DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use DD/MM/YYYY" });
                }

                // Get doctor's shifts for this day
                var doctorEntity = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.User.Email == doctor);

                if (doctorEntity == null)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                var shifts = await _context.Doctor_Shifts
                    .Include(ds => ds.Doctor)
                    .Where(ds => ds.DoctorId == doctorEntity.Id && ds.ShiftDate.Date == parsedDate.Date)
                    .Select(ds => ds.ShiftType)
                    .ToListAsync();

                return Ok(new { shifts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/chatbot/available-dates
        [HttpGet("available-dates")]
        public async Task<ActionResult<object>> GetAvailableDates([FromQuery] string doctor)
        {
            try
            {
                if (string.IsNullOrEmpty(doctor))
                {
                    return BadRequest(new { message = "Doctor name is required" });
                }

                var doctorEntity = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.User.Email == doctor);

                if (doctorEntity == null)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                // Lấy tất cả ngày làm việc của bác sĩ trong tương lai (30 ngày tới)
                var workingDates = await _context.Doctor_Shifts
                    .Where(ds => ds.DoctorId == doctorEntity.Id && 
                                ds.ShiftDate.Date >= DateTime.Today)
                    .Select(ds => ds.ShiftDate.Date)
                    .Distinct()
                    .OrderBy(date => date)
                    .Take(30)
                    .ToListAsync();

                // Lấy ngày nghỉ đã được phê duyệt từ ShiftRequest
                var approvedLeaveDates = await _context.ShiftRequests
                    .Where(sr => sr.DoctorId == doctorEntity.Id && 
                                sr.RequestType == "Leave" && 
                                sr.Status == "Approved" &&
                                sr.Shift.ShiftDate.Date >= DateTime.Today)
                    .Select(sr => sr.Shift.ShiftDate.Date)
                    .Distinct()
                    .ToListAsync();

                // Loại bỏ ngày nghỉ khỏi ngày làm việc
                var availableWorkingDates = workingDates.Except(approvedLeaveDates).ToList();

                // Tạo danh sách ngày với thông tin chi tiết
                var dateInfo = new List<object>();
                
                foreach (var date in availableWorkingDates.Take(15)) // Hiển thị 15 ngày gần nhất
                {
                    dateInfo.Add(new
                    {
                        date = date.ToString("dd/MM/yyyy"),
                        type = "working",
                        displayText = $"📅 {date.ToString("dd/MM/yyyy")} - Có lịch làm việc"
                    });
                }

                // Thêm ngày nghỉ đã được phê duyệt
                foreach (var date in approvedLeaveDates.Take(10))
                {
                    dateInfo.Add(new
                    {
                        date = date.ToString("dd/MM/yyyy"),
                        type = "leave",
                        displayText = $"🚫 {date.ToString("dd/MM/yyyy")} - Nghỉ phép"
                    });
                }

                return Ok(new { 
                    availableDates = availableWorkingDates.Select(d => d.ToString("dd/MM/yyyy")).ToList(),
                    dateInfo = dateInfo.OrderBy(x => ((dynamic)x).date).ToList(),
                    workingDates = availableWorkingDates.Select(d => d.ToString("dd/MM/yyyy")).ToList(),
                    leaveDates = approvedLeaveDates.Select(d => d.ToString("dd/MM/yyyy")).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/chatbot/appointments
        [HttpPost("appointments")]
        public async Task<ActionResult<object>> CreateAppointment([FromBody] AppointmentRequest request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(request.Department) ||
                    string.IsNullOrEmpty(request.Doctor) ||
                    string.IsNullOrEmpty(request.Service) ||
                    string.IsNullOrEmpty(request.Date) ||
                    string.IsNullOrEmpty(request.Shift) ||
                    request.PatientId == null || !request.PatientId.HasValue)
                {
                    return BadRequest(new { message = "All required fields must be provided" });
                }

                // Parse date
                if (!DateTime.TryParseExact(request.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use DD/MM/YYYY" });
                }

                // Find patient
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == request.PatientId.Value && p.Status == PatientStatus.Active);

                if (patient == null)
                {
                    return BadRequest(new { message = "Patient not found" });
                }

                // Find service
                var service = await _context.Services
                    .FirstOrDefaultAsync(s => s.Name == request.Service);

                if (service == null)
                {
                    return BadRequest(new { message = "Service not found" });
                }

                // Find clinic (assuming first clinic for now)
                var clinic = await _context.Clinics.FirstOrDefaultAsync();
                if (clinic == null)
                {
                    return BadRequest(new { message = "No clinic available" });
                }

                // Generate appointment code
                var appointmentCode = "GC-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

                // Create new appointment
                var appointment = new Appointment
                {
                    AppointmentDate = parsedDate,
                    Shift = request.Shift,
                    Note = request.Notes ?? "Không có ghi chú",
                    Status = AppointmentStatus.Scheduled,
                    PatientId = patient.Id,
                    ClinicId = clinic.Id,
                    ServiceId = service.Id
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // Create doctor appointment relationship
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.User.Email == request.Doctor);

                if (doctor != null)
                {
                    var doctorAppointment = new Doctor_Appointment
                    {
                        DoctorId = doctor.Id,
                        AppointmentId = appointment.Id,
                        Status = DoctorAppointmentStatus.Assigned
                    };
                    _context.Doctor_Appointments.Add(doctorAppointment);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { 
                    success = true,
                    appointmentCode = appointmentCode,
                    message = "Appointment created successfully",
                    appointmentId = appointment.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/chatbot/patient-from-user
        [HttpGet("patient-from-user")]
        public async Task<ActionResult<object>> GetPatientFromUser()
        {
            try
            {
                // Get user ID from JWT token (you'll need to implement this based on your auth system)
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                // Find patient associated with this user
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .Where(p => p.UserId == int.Parse(userId) && p.Status == PatientStatus.Active)
                    .Select(p => new
                    {
                        p.Id,
                        Name = p.User.Email, // Use email as name since User doesn't have Name
                        p.Phone,
                        p.Dob,
                        p.CCCD,
                        p.Gender,
                        p.Address
                    })
                    .FirstOrDefaultAsync();

                if (patient == null)
                {
                    return NotFound(new { message = "Patient data not found for this user" });
                }

                return Ok(new { patient });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/chatbot/search-patient
        [HttpGet("search-patient")]
        public async Task<ActionResult<object>> SearchPatient([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term) || term.Length < 5)
                {
                    return BadRequest(new { message = "Search term must be at least 5 characters" });
                }

                // Search by phone number or CCCD
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .Where(p => p.Status == PatientStatus.Active &&
                               (p.Phone == term || p.CCCD == term))
                    .Select(p => new
                    {
                        p.Id,
                        Name = p.User.Email, // Use email as name
                        p.Phone,
                        p.Dob,
                        p.CCCD,
                        p.Gender,
                        p.Address
                    })
                    .FirstOrDefaultAsync();

                if (patient == null)
                {
                    return NotFound(new { message = "Patient not found" });
                }

                return Ok(new { patient });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/chatbot/create-patient
        [HttpPost("create-patient")]
        public async Task<ActionResult<object>> CreatePatient([FromBody] CreatePatientRequest request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(request.Name) ||
                    string.IsNullOrEmpty(request.Phone) ||
                    string.IsNullOrEmpty(request.CCCD) ||
                    request.Dob == null)
                {
                    return BadRequest(new { message = "All required fields must be provided" });
                }

                // Check if patient already exists
                var existingPatient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Phone == request.Phone || p.CCCD == request.CCCD);

                if (existingPatient != null)
                {
                    return BadRequest(new { message = "Patient with this phone number or CCCD already exists" });
                }

                // Create new user first
                var user = new User
                {
                    Email = request.Name, // Use name as email temporarily
                    Password = "temp123", // Temporary password
                    Status = UserStatus.Active
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create new patient
                var patient = new Patient
                {
                    Phone = request.Phone,
                    Dob = request.Dob,
                    CCCD = request.CCCD,
                    Gender = request.Gender == "Male" ? Gender.Male : Gender.Female,
                    Address = request.Address ?? "",
                    Status = PatientStatus.Active,
                    UserId = user.Id
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    patient = new
                    {
                        patient.Id,
                        Name = user.Email,
                        patient.Phone,
                        patient.Dob,
                        patient.CCCD,
                        patient.Gender,
                        patient.Address
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // Helper method to get current user ID from JWT token
        private string GetCurrentUserId()
        {
            // This is a placeholder - you need to implement this based on your authentication system
            // For now, we'll return null to indicate no user is authenticated
            // In a real implementation, you would extract the user ID from the JWT token
            
            // Example implementation (you'll need to adapt this to your auth system):
            /*
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userClaim?.Value;
            */
            
            return null; // Placeholder
        }
    }

    public class AppointmentRequest
    {
        public string PatientName { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string Doctor { get; set; }
        public string Service { get; set; }
        public string AppointmentDate { get; set; }
        public string AppointmentShift { get; set; }
        public string Notes { get; set; }
        public int? PatientId { get; set; }
        public string Date { get; set; }
        public string Shift { get; set; }
    }

    public class CreatePatientRequest
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CCCD { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
} 