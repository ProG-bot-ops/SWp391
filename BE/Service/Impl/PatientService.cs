using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using SWP391_SE1914_ManageHospital.Ultility.Validation;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDBContext _context;
        private readonly IpatientMapper _mapper;

        public PatientService(ApplicationDBContext context, IpatientMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> CheckUniqueCodeAsync()
        {
            string newCode;
            do
            {
                newCode = "PAT" + Guid.NewGuid().ToString("N")[..5].ToUpper();
            } while (await _context.Patients.AnyAsync(p => p.Code == newCode));
            return newCode;
        }

        public async Task<PatientRespone> CreatePatientAsync(PatientCreate create)
        {
            if (create.UserId > 0)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == create.UserId);
                if (!userExists)
                {
                    throw new Exception($"User with Id {create.UserId} does not exist.");
                }
            }
            Patient entity = _mapper.CreateToEntity(create);

            await _context.Patients.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.EntityToRespone(entity);
        }


        public async Task<PatientRespone> FindPatientByIdAsync(int id)
        {
            var patient =  await _context.Patients.FindAsync(id);
            if (patient is null)
            {
                throw new Exception($"Can not find patient id: {id}");
            }
            return _mapper.EntityToRespone(patient);
        }

        public async Task<PatientRespone> FindPatientByUserIdAsync(int id)
        {
            var coID = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == id)
                ?? throw new Exception($"Không thể tìm Bệnh nhân có user id: {id}");
            return _mapper.EntityToRespone(coID);
        }

        public async Task<IEnumerable<PatientRespone>> GetAllPatientAsync()
        {
            var patients = await _context.Patients.ToListAsync();
            if (patients is null)
            {
                throw new Exception("No Patient");
            }

            return _mapper.ListEntityToRespone(patients);
        }

        public async Task<bool> HardDeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient is null)
            {
                throw new Exception($"Can not find patient ID: {id}");
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        

        public async Task<IEnumerable<PatientInfoAdmin>> PatientInfoAdAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Medical_Record)
                    .ThenInclude(mr => mr.Doctor)
                .Include(a => a.Invoice)
                .ToListAsync();

            if (!appointments.Any())
            {
                throw new Exception("Không tìm thấy lịch sử khám của bệnh nhân.");
            }

            var result = _mapper.PatientInfoAdmins(appointments);
            return result;
        }


        public async Task<IEnumerable<PatientRespone>> SearchPatientByKeyAsync(string key)
        {
            var result = await _context.Patients
                .Where(p => p.Name.Contains(key))
                .ToListAsync();

            if (!result.Any())
            {
                throw new Exception($"Can not find patient with key: \"{key}\".");
            }

            return _mapper.ListEntityToRespone(result);
        }

        public async Task<PatientRespone> SoftDeletePatientColorAsync(int id, Status.PatientStatus newStatus)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient is null)
            {
                throw new Exception($"Can not fint patient with ID: {id}");
            }

            patient.Status = newStatus;
            patient.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return _mapper.EntityToRespone(patient);
        }

        public async Task<PatientRespone> UpdatePatientAsync(int id, PatientUpdate update)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient is null)
            {
                throw new Exception($"Can not find patient with ID: {id}");
            }

            // Kiểm tra CCCD và Phone có bị trùng với bệnh nhân khác không
            if (await _context.Patients.AnyAsync(p => p.Id != id && p.CCCD == update.CCCD))
            {
                throw new Exception("CCCD đã tồn tại trong hệ thống");
            }

            if (await _context.Patients.AnyAsync(p => p.Id != id && p.Phone == update.Phone))
            {
                throw new Exception("Số điện thoại đã tồn tại trong hệ thống");
            }

            // Cập nhật thông tin bệnh nhân
            patient.Name = update.Name?.Trim();
            patient.Gender = update.Gender;
            patient.Dob = update.Dob;
            patient.CCCD = update.CCCD?.Trim();
            patient.Phone = update.Phone?.Trim();
            patient.EmergencyContact = update.EmergencyContact?.Trim();
            patient.Address = update.Address?.Trim();
            patient.InsuranceNumber = update.InsuranceNumber?.Trim();
            patient.Allergies = update.Allergies?.Trim();
            patient.BloodType = update.BloodType?.Trim();
            patient.ImageURL = update.ImageURL?.Trim();
            patient.Status = update.Status;
            patient.UpdateBy = update.UpdateBy ?? "System";
            patient.UpdateDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật bệnh nhân: {ex.Message}");
            }

            return _mapper.EntityToRespone(patient);
        }

        public async Task<PatientRespone> UpdatePatientByUserIdAsync(int userId, PatientUpdate update)
        {
            var coID = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId)
                ?? throw new Exception($"Không thể tìm Bệnh nhân có user id: {userId}");

            PatientUpdateValidator.Validate(update);

            coID.Name = update.Name.Trim();
            coID.Gender = update.Gender;
            coID.Dob = update.Dob;
            coID.Code = update.Code;
            coID.CCCD = update.CCCD;
            coID.Phone = update.Phone;
            coID.EmergencyContact = update.EmergencyContact;
            coID.Address = update.Address;
            coID.InsuranceNumber = update.InsuranceNumber;
            coID.Allergies = update.Allergies;
            coID.BloodType = update.BloodType;
            coID.ImageURL = update.ImageURL;
            coID.Status = update.Status;
            coID.UpdateBy = update.Name;
            coID.UpdateDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return _mapper.EntityToRespone(coID);

        }

        public async Task<PatientRespone> UpdatePatientImageAsync(int userId, PatientImageUpdate imageurl)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId)
                ?? throw new Exception($"Không thể tìm Bệnh nhân có user id: {userId}");

            patient.ImageURL = imageurl.ImageURL;
            await _context.SaveChangesAsync();

            return _mapper.EntityToRespone(patient);
        }

        // New methods for appointment system
        public async Task<SWP391_SE1914_ManageHospital.Models.Entities.Patient?> SearchPatientByPhoneOrCCCDAsync(string? phone, string? cccd)
        {
            Console.WriteLine($"=== SEARCH PATIENT DEBUG ===");
            Console.WriteLine($"Phone: '{phone}'");
            Console.WriteLine($"CCCD: '{cccd}'");
            
            if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(cccd))
            {
                Console.WriteLine("Both phone and cccd are null or empty");
                return null;
            }

            var trimmedPhone = phone?.Trim();
            var trimmedCCCD = cccd?.Trim();

            Console.WriteLine($"Looking for phone='{trimmedPhone}' OR cccd='{trimmedCCCD}'");

            try
            {
                // Đầu tiên, lấy tất cả bệnh nhân để debug
                var allPatients = await _context.Patients
                    .Select(p => new { p.Id, p.Name, p.Phone, p.CCCD })
                    .Take(5)
                    .ToListAsync();
                
                Console.WriteLine($"Sample patients in DB:");
                foreach (var p in allPatients)
                {
                    Console.WriteLine($"  ID={p.Id}, Name='{p.Name}', Phone='{p.Phone}', CCCD='{p.CCCD}'");
                }

                // Tìm kiếm với logic linh hoạt hơn
                var patientData = await _context.Patients
                    .Where(p => 
                        (!string.IsNullOrWhiteSpace(trimmedPhone) && 
                         p.Phone != null && 
                         (p.Phone.Trim() == trimmedPhone || p.Phone.Contains(trimmedPhone))) ||
                        (!string.IsNullOrWhiteSpace(trimmedCCCD) && 
                         p.CCCD != null && 
                         (p.CCCD.Trim() == trimmedCCCD || p.CCCD.Contains(trimmedCCCD)))
                    )
                    .Select(p => new
                    {
                        p.Id,
                        Name = p.Name ?? "N/A",
                        Phone = p.Phone ?? "N/A",
                        CCCD = p.CCCD ?? "N/A",
                        Address = p.Address ?? "N/A",
                        p.Dob,
                        p.Gender,
                        p.UserId,
                        Code = p.Code ?? "N/A",
                        p.Status,
                        ImageURL = p.ImageURL ?? "N/A",
                        p.CreateDate,
                        p.UpdateDate,
                        CreateBy = p.CreateBy ?? "N/A",
                        UpdateBy = p.UpdateBy ?? "N/A"
                    })
                    .FirstOrDefaultAsync();

                if (patientData == null)
                {
                    Console.WriteLine("Patient not found with flexible search");
                    
                    // Thử tìm kiếm chính xác hơn
                    var exactMatch = await _context.Patients
                        .Where(p => 
                            (!string.IsNullOrWhiteSpace(trimmedPhone) && p.Phone == trimmedPhone) ||
                            (!string.IsNullOrWhiteSpace(trimmedCCCD) && p.CCCD == trimmedCCCD)
                        )
                        .Select(p => new { p.Id, p.Name, p.Phone, p.CCCD })
                        .FirstOrDefaultAsync();
                    
                    if (exactMatch != null)
                    {
                        Console.WriteLine($"Found exact match: ID={exactMatch.Id}, Phone='{exactMatch.Phone}', CCCD='{exactMatch.CCCD}'");
                    }
                    else
                    {
                        Console.WriteLine("No exact match found either");
                    }
                    
                    Console.WriteLine($"=== END SEARCH DEBUG ===");
                    return null;
                }

                // Lấy thông tin User riêng biệt nếu cần
                string? userEmail = null;
                if (patientData.UserId > 0)
                {
                    try
                    {
                        userEmail = await _context.Users
                            .Where(u => u.Id == patientData.UserId)
                            .Select(u => u.Email ?? "N/A")
                            .FirstOrDefaultAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting user email: {ex.Message}");
                        userEmail = "N/A";
                    }
                }

                // Tạo Patient object với thông tin đã lấy
                var result = new Patient
                {
                    Id = patientData.Id,
                    Name = patientData.Name,
                    Phone = patientData.Phone,
                    CCCD = patientData.CCCD,
                    Address = patientData.Address,
                    Dob = patientData.Dob,
                    Gender = patientData.Gender,
                    UserId = patientData.UserId,
                    Code = patientData.Code,
                    Status = patientData.Status,
                    ImageURL = patientData.ImageURL,
                    CreateDate = patientData.CreateDate,
                    UpdateDate = patientData.UpdateDate,
                    CreateBy = patientData.CreateBy,
                    UpdateBy = patientData.UpdateBy,
                    User = userEmail != null && userEmail != "N/A" ? new User { Id = patientData.UserId, Email = userEmail } : null
                };

                Console.WriteLine($"Search result: Found patient ID={result.Id}, Name={result.Name}");
                Console.WriteLine($"=== END SEARCH DEBUG ===");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchPatientByPhoneOrCCCDAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine($"=== END SEARCH DEBUG ===");
                return null;
            }
        }

        public async Task<object> CreatePatientWithUserAsync(PatientCreateRequest request)
        {
            // Kiểm tra CCCD và Phone đã tồn tại chưa
            if (await _context.Patients.AnyAsync(p => p.CCCD == request.CCCD))
            {
                throw new Exception("CCCD đã tồn tại trong hệ thống");
            }

            if (await _context.Patients.AnyAsync(p => p.Phone == request.Phone))
            {
                throw new Exception("Số điện thoại đã tồn tại trong hệ thống");
            }

            // Tạo mật khẩu ngẫu nhiên
            var password = GenerateRandomPassword();

            // Tạo User mới với email từ phone number
            var user = new User
            {
                Email = $"{request.Phone}@hospital.com", // Tạo email từ số điện thoại
                Password = password, // Trong thực tế cần hash password
                Status = UserStatus.Active
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(); // Lưu để lấy UserId

            // Tạo Patient mới
            var patient = new Patient
            {
                Name = request.FullName,
                Phone = request.Phone,
                CCCD = request.CCCD,
                Address = request.Address,
                Dob = request.Dob,
                Gender = (Status.Gender)request.Gender, // Convert int to enum
                UserId = user.Id,
                Code = await CheckUniqueCodeAsync(),
                Status = PatientStatus.Active,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "System",
                UpdateBy = "System",
                BloodType = request.BloodType,
                InsuranceNumber = request.InsuranceNumber,
                Allergies = request.Allergies,
                EmergencyContact = request.EmergencyContact
            };

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            return new
            {
                success = true,
                patientId = patient.Id,
                userId = user.Id,
                message = "Tạo bệnh nhân thành công"
            };
        }

        public async Task<object> CreatePatientOnlyAsync(PatientCreateRequest request)
        {
            // Kiểm tra CCCD và Phone đã tồn tại chưa
            if (await _context.Patients.AnyAsync(p => p.CCCD == request.CCCD))
            {
                throw new Exception("CCCD đã tồn tại trong hệ thống");
            }

            if (await _context.Patients.AnyAsync(p => p.Phone == request.Phone))
            {
                throw new Exception("Số điện thoại đã tồn tại trong hệ thống");
            }

            // Tạo Patient mới với admin user (UserId = 1)
            var patient = new Patient
            {
                Name = request.FullName,
                Phone = request.Phone,
                CCCD = request.CCCD,
                Address = request.Address,
                Dob = request.Dob,
                Gender = (Status.Gender)request.Gender, // Convert int to enum
                UserId = 1, // Sử dụng admin user
                Code = await CheckUniqueCodeAsync(),
                Status = PatientStatus.Active,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "System",
                UpdateBy = "System",
                BloodType = request.BloodType,
                InsuranceNumber = request.InsuranceNumber,
                Allergies = request.Allergies,
                EmergencyContact = request.EmergencyContact
            };

            await _context.Patients.AddAsync(patient);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException;
                var errorMessage = $"Database update error: {dbEx.Message}";
                if (innerException != null)
                {
                    errorMessage += $" Inner exception: {innerException.Message}";
                }
                throw new Exception($"Lỗi khi lưu bệnh nhân: {errorMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi không xác định khi tạo bệnh nhân: {ex.Message}");
            }

            return new
            {
                success = true,
                data = new
                {
                    id = patient.Id,
                    name = patient.Name,
                    phone = patient.Phone,
                    cccd = patient.CCCD,
                    address = patient.Address,
                    birthDate = patient.Dob.ToString("yyyy-MM-dd"),
                    gender = patient.Gender.ToString()
                },
                message = "Tạo bệnh nhân thành công"
            };
        }

        public async Task<object> CreatePatientAndAppointmentAsync(PatientAndAppointmentCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validation input
                if (request == null)
                    throw new Exception("Dữ liệu yêu cầu không hợp lệ!");

                if (string.IsNullOrEmpty(request.FullName))
                    throw new Exception("Tên bệnh nhân không được để trống!");

                if (string.IsNullOrEmpty(request.Phone))
                    throw new Exception("Số điện thoại không được để trống!");

                if (string.IsNullOrEmpty(request.CCCD))
                    throw new Exception("CCCD không được để trống!");

                if (string.IsNullOrEmpty(request.EmergencyContact))
                    throw new Exception("Liên hệ khẩn cấp không được để trống!");

                if (string.IsNullOrEmpty(request.Email))
                    throw new Exception("Email không được để trống!");

                // Validate phone format
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^0(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$"))
                    throw new Exception("Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số!");

                // Validate emergency contact format
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.EmergencyContact, @"^0(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$"))
                    throw new Exception("Liên hệ khẩn cấp phải bắt đầu bằng 0 và có 10 chữ số!");

                if (request.AppointmentDate.Date < DateTime.Now.Date)
                    throw new Exception("Không thể đặt lịch cho ngày trong quá khứ!");

                if (string.IsNullOrEmpty(request.Shift) || (request.Shift.ToLower() != "morning" && request.Shift.ToLower() != "afternoon"))
                    throw new Exception("Ca làm việc không hợp lệ! Chỉ chấp nhận 'morning' hoặc 'afternoon'");

                // Kiểm tra CCCD và Phone đã tồn tại chưa
                if (await _context.Patients.AnyAsync(p => p.CCCD == request.CCCD))
                {
                    throw new Exception("CCCD đã tồn tại trong hệ thống");
                }

                if (await _context.Patients.AnyAsync(p => p.Phone == request.Phone))
                {
                    throw new Exception("Số điện thoại đã tồn tại trong hệ thống");
                }

                // Tạo Patient mới
                var patient = new Patient
                {
                    Name = request.FullName,
                    Phone = request.Phone,
                    CCCD = request.CCCD,
                    Address = request.Address,
                    Dob = request.Dob,
                    Gender = (Status.Gender)request.Gender,
                    UserId = 1, // Admin ID as requested
                    Code = await CheckUniqueCodeAsync(),
                    Status = PatientStatus.Active,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreateBy = "Admin",
                    UpdateBy = "Admin"
                };

                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();

                // Tạo mã lịch hẹn tự động
                string appointmentCode = $"APT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                // Tạo Invoice trước với AppointmentId tạm thời
                var invoice = new Invoice
                {
                    Name = $"Hóa đơn - {patient.Name}",
                    Code = $"INV-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    InitialAmount = 0,
                    DiscountAmount = 0,
                    TotalAmount = 0,
                    Notes = $"Hóa đơn cho cuộc hẹn {appointmentCode}",
                    Status = InvoiceStatus.Unpaid,
                    PatientId = patient.Id,
                    AppointmentId = 0, // Tạm thời, sẽ được cập nhật sau
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreateBy = patient.Name,
                    UpdateBy = patient.Name
                };

                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                // Tạo Appointment với cùng ID như Invoice (1:1 relationship)
                var appointment = new Appointment
                {
                    Id = invoice.Id, // Sử dụng cùng ID với Invoice
                    Name = $"Lịch hẹn - {patient.Name}",
                    Code = appointmentCode,
                    AppointmentDate = request.AppointmentDate.Date,
                    StartTime = null,
                    EndTime = null,
                    Shift = request.Shift,
                    Status = AppointmentStatus.Scheduled,
                    Note = !string.IsNullOrEmpty(request.Note) ? request.Note : "Cuộc hẹn mới",
                    isSend = false,
                    PatientId = patient.Id,
                    ClinicId = request.ClinicId,
                    ReceptionId = null,
                    ServiceId = request.ServiceId,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreateBy = patient.Name,
                    UpdateBy = patient.Name
                };

                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();

                // Cập nhật Invoice với AppointmentId chính xác
                invoice.AppointmentId = appointment.Id;
                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();

                // Liên kết với doctor
                var doctorAppointment = new Doctor_Appointment
                {
                    DoctorId = request.DoctorId,
                    AppointmentId = appointment.Id,
                    Status = DoctorAppointmentStatus.Assigned
                };

                await _context.Doctor_Appointments.AddAsync(doctorAppointment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new
                {
                    success = true,
                    data = new
                    {
                        patient = new
                        {
                            id = patient.Id,
                            name = patient.Name,
                            phone = patient.Phone,
                            cccd = patient.CCCD,
                            address = patient.Address,
                            birthDate = patient.Dob.ToString("yyyy-MM-dd"),
                            gender = patient.Gender.ToString()
                        },
                        appointment = new
                        {
                            id = appointment.Id,
                            code = appointment.Code,
                            date = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                            shift = appointment.Shift,
                            status = appointment.Status.ToString()
                        }
                    },
                    message = "Tạo bệnh nhân và cuộc hẹn thành công"
                };
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                var innerException = dbEx.InnerException;
                var errorMessage = $"Database update error: {dbEx.Message}";
                if (innerException != null)
                {
                    errorMessage += $" Inner exception: {innerException.Message}";
                }
                throw new Exception($"Lỗi khi tạo bệnh nhân và cuộc hẹn: {errorMessage}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi tạo bệnh nhân và cuộc hẹn: {ex.Message}");
            }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private Status.Gender ParseGender(string? genderStr)
        {
            if (string.IsNullOrWhiteSpace(genderStr))
                return Status.Gender.Male; // Default to Male instead of Other

            return genderStr.ToLower() switch
            {
                "male" => Status.Gender.Male,
                "female" => Status.Gender.Female,
                _ => Status.Gender.Male // Default to Male instead of Other
            };
        }

        public async Task<object> DebugSearchAsync(string? phone, string? cccd)
        {
            var allPatients = await _context.Patients
                .Select(p => new { p.Id, p.Name, p.Phone, p.CCCD })
                .Take(10)
                .ToListAsync();

            var searchResults = new List<object>();

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var phoneMatches = await _context.Patients
                    .Where(p => p.Phone == phone)
                    .Select(p => new { p.Id, p.Name, p.Phone, p.CCCD })
                    .ToListAsync();
                searchResults.AddRange(phoneMatches);
            }

            if (!string.IsNullOrWhiteSpace(cccd))
            {
                var cccdMatches = await _context.Patients
                    .Where(p => p.CCCD == cccd)
                    .Select(p => new { p.Id, p.Name, p.Phone, p.CCCD })
                    .ToListAsync();
                searchResults.AddRange(cccdMatches);
            }

            return new
            {
                searchParams = new { phone, cccd },
                allPatients = allPatients,
                searchResults = searchResults,
                totalPatients = await _context.Patients.CountAsync()
            };
        }

        public async Task<object> GetAllPatientsForDebugAsync()
        {
            var patients = await _context.Patients
                .Select(p => new { p.Id, p.Name, p.Phone, p.CCCD, p.CreateDate })
                .OrderBy(p => p.Id)
                .ToListAsync();

            return new
            {
                totalPatients = patients.Count,
                patients = patients,
                nextId = patients.Any() ? patients.Max(p => p.Id) + 1 : 1
            };
        }

        // Method để reset auto-increment sequence
        public async Task<object> ResetPatientAutoIncrementAsync()
        {
            try
            {
                // Lấy ID cao nhất hiện tại
                var maxId = await _context.Patients.MaxAsync(p => (int?)p.Id) ?? 0;
                
                // Reset auto-increment về maxId + 1
                var resetQuery = $"ALTER TABLE patients AUTO_INCREMENT = {maxId + 1}";
                await _context.Database.ExecuteSqlRawAsync(resetQuery);
                
                return new
                {
                    success = true,
                    message = $"Đã reset auto-increment về {maxId + 1}",
                    currentMaxId = maxId,
                    nextId = maxId + 1
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = $"Lỗi khi reset auto-increment: {ex.Message}"
                };
            }
        }

        // Method để kiểm tra sequence hiện tại
        public async Task<object> GetPatientSequenceInfoAsync()
        {
            try
            {
                var patients = await _context.Patients
                    .Select(p => p.Id)
                    .OrderBy(id => id)
                    .ToListAsync();

                var maxId = patients.Any() ? patients.Max() : 0;
                var minId = patients.Any() ? patients.Min() : 0;
                var gaps = new List<string>();

                // Tìm các khoảng trống trong ID
                for (int i = 1; i <= maxId; i++)
                {
                    if (!patients.Contains(i))
                    {
                        gaps.Add(i.ToString());
                    }
                }

                return new
                {
                    totalPatients = patients.Count,
                    minId = minId,
                    maxId = maxId,
                    nextExpectedId = maxId + 1,
                    gaps = gaps,
                    allIds = patients
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = $"Lỗi khi kiểm tra sequence: {ex.Message}"
                };
            }
        }
    }
}
