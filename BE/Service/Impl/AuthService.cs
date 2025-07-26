using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authenication;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

public class AuthService : IAuthService
{
    private readonly ApplicationDBContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    public AuthService(ApplicationDBContext context, IPasswordHasher passwordHasher, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<bool> ChangePasswordAsync(int userId, UserType userType, string oldPassword, string newPassword, string confirmPassword)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng.");

            if (!_passwordHasher.VerifyPassword(oldPassword, user.Password))
                throw new Exception("Mật khẩu cũ không đúng!");

            if (newPassword != confirmPassword)
                throw new Exception("Mật khẩu mới và xác nhận không trùng khớp.");

            user.Password = _passwordHasher.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi khi đổi mật khẩu");
        }  
    }

    public async Task<bool> ForgotPasswordAsync(string email, UserType userType)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Patients)
                .Include(u => u.Doctors)
                .Include(u => u.Nurses)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) throw new Exception("Email không tồn tại!");

            var roles = await _context.User_Roles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name.Trim().ToLower())
                .ToListAsync();

            if (!roles.Contains(userType.ToString().ToLower()))
                throw new Exception("Email không đúng!");
            var resetToken = Guid.NewGuid().ToString(); 
            var resetTokenExpiryTime = DateTime.Now.AddHours(1); // Token có hiệu lực trong 1 giờ
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiryTime = resetTokenExpiryTime; // Token 1h

            await _context.SaveChangesAsync();
            await _emailService.SendResetPasswordEmailAsync(email, resetToken, userType.ToString());

            return true;
        } 
        catch (Exception ex)
        {
            // Xử lý lỗi nếu cần
            throw new Exception("Quên mật khẩu không thành công", ex);
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequestDTO request)
    {
        var user = await _context.Users
        .Include(u => u.User_Roles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            throw new Exception("Email hoặc mật khẩu không đúng");
        }

        if (user.Status != UserStatus.Active)
        {
            throw new Exception("Tài khoản đã bị khóa");
        }

        // Kiểm tra người dùng có role phù hợp không
        var matchedRole = user.User_Roles
            .Select(ur => ur.Role.Name)
            .FirstOrDefault(roleName => roleName == request.UserType.ToString());

        if (matchedRole == null)
        {
            throw new Exception($"Người dùng không có quyền truy cập với vai trò {request.UserType}");
        }
        else
        {
            Console.WriteLine($"Xin chào {request.UserType}"); // Debug log
        }

            // Tạo JWT và Refresh Token
            var token = GenerateJwtToken(user, request.UserType); // Cần truyền enum để tạo claim đúng
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        // Mapping Redirect URL theo role
        string redirectUrl = request.UserType switch
        {
            UserType.Admin => "/frontend/dashboard/index.html",
            UserType.Doctor => "/frontend/dashboard/doctor-page.html",
            UserType.Nurse => "/frontend/frontend/nurse-ui.html",
            UserType.Patient => "/frontend/index.html",
            _ => "/"
        };

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddHours(1),
            UserInfo = CreateUserInfo(user, request.UserType),
            RedirectUrl = redirectUrl
        };
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private string GenerateJwtToken(User user, UserType userType)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("UserType", userType.ToString())
    };

        // Lấy FullName
        string fullName = userType switch
        {
            UserType.Admin => "/html/dashboard/index.html",
            UserType.Doctor => "/html/frontend/doctor-ui.html",
            UserType.Nurse => "/html/frontend/nurse-ui.html",
            UserType.Patient => "/html/frontend/index.html",
            _ => "/"
        };

        if (!string.IsNullOrEmpty(fullName))
        {
            claims.Add(new Claim(ClaimTypes.Name, fullName));
        }

        // Thêm các role
        foreach (var role in user.User_Roles.Select(ur => ur.Role.Name))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:TokenExpirationInHours"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<bool> LogoutAsync(int userId, UserType userType)
    {
        try
        {
            if (userType == UserType.Patient)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null; // Xóa RefreshToken
                    user.RefreshTokenExpiryTime = null; // Xóa thời gian hết hạn RefreshToken
                }
                else
                {
                    return false; // Người dùng không tồn tại
                }
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu cần
            throw new Exception("Đăng xuất không thành công", ex);
        }
    }
    
    private UserInfo CreateUserInfo(User user, UserType userType)
    {
        var userInfo = new UserInfo
        {
            Id = user.Id,
            Email = user.Email,
            UserType = userType,
            Roles = user.User_Roles.Select(ur => ur.Role.Name).ToList()
        };

        // Chỉ xử lý FullName, Code nếu có bảng thật sự
        switch (userType)
        {
            case UserType.Doctor:
                var doctor = user.Doctors.FirstOrDefault();
                if (doctor != null)
                {
                    userInfo.FullName = doctor.Name;
                    userInfo.Code = doctor.Code;
                }
                break;

            case UserType.Patient:
                var patientPatient = user.Patients.OrderBy(p => p.Id).FirstOrDefault();
                if (patientPatient != null)
                {
                    userInfo.FullName = patientPatient.Name;
                    userInfo.Code = patientPatient.Code;
                }
                break;

            case UserType.Nurse:
                var nurse = user.Nurses.FirstOrDefault();
                if (nurse != null)
                {
                    userInfo.FullName = nurse.Name;
                    userInfo.Code = nurse.Code;
                }
                break;

            case UserType.Admin:
                userInfo.FullName = user.Email; 
                userInfo.Code = "ADMIN";
                break;

            default:
                userInfo.FullName = "Unknown";
                userInfo.Code = "N/A";
                break;
        }

        return userInfo;
    }
    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {

        var user = await _context.Users
        .Include(u => u.User_Roles).ThenInclude(ur => ur.Role)
        .Include(u => u.Doctors)
        .Include(u => u.Patients)
        .Include(u => u.Nurses)
        .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new Exception("Refresh token không hợp lệ hoặc đã hết hạn");
        }

        // Xác định user type từ roles
        var roleNames = user.User_Roles.Select(ur => ur.Role.Name.ToLower()).ToList();

        UserType userType;
        if (roleNames.Contains("doctor")) userType = UserType.Doctor;
        else if (roleNames.Contains("nurse")) userType = UserType.Nurse;
        else if (roleNames.Contains("patient")) userType = UserType.Patient;
        else if (roleNames.Contains("admin")) userType = UserType.Admin;
        else throw new Exception("Không thể xác định loại người dùng");

        var newAccessToken = GenerateJwtToken(user, userType);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenExpiration = DateTime.UtcNow.AddHours(1),
            UserInfo = CreateUserInfo(user, userType),
            RedirectUrl = userType switch
            {
                UserType.Admin => "/html/dashboard/index.html",
                UserType.Doctor => "/html/frontend/doctor-ui.html",
                UserType.Nurse => "/html/frontend/nurse-ui.html",
                UserType.Patient => "/html/frontend/index.html",
                _ => "/"
            }
        };
    }
    public async Task<string> CheckUniqueCodeAsync()
    {
        string newCode;
        bool isExist;

        do
        {
            newCode = GenerateCode.GeneratePatientCode();
            _context.ChangeTracker.Clear();
            isExist = await _context.Patients.AnyAsync(p => p.Code == newCode);
        }
        while (isExist);

        return newCode;
    }
    public async Task<PatientRegisterResponse> RegisterPatientAsync(PatientRegisterRequest request)
    {
        try
        {
            Console.WriteLine($"=== AUTH SERVICE: RegisterPatientAsync ===");
            Console.WriteLine($"Email: {request.Email}");
            Console.WriteLine($"FullName: {request.FullName}");
            Console.WriteLine($"Code: {request.Code}");
            Console.WriteLine($"Gender: {request.Gender}");
            Console.WriteLine($"Dob: {request.Dob}");
            Console.WriteLine($"CCCD: {request.CCCD}");
            Console.WriteLine($"Phone: {request.Phone}");
            Console.WriteLine($"EmergencyContact: {request.EmergencyContact}");
            Console.WriteLine($"Address: {request.Address}");

            //  Kiểm tra Email tồn tại chưa
            if (await _context.Users.AnyAsync(x => x.Email == request.Email))
                throw new Exception("Email đã tồn tại!");

            //  Kiểm tra Phone và CCCD
            if (await _context.Patients.AnyAsync(x => x.Phone == request.Phone))
                throw new Exception("Số điện thoại đã tồn tại");

            if (await _context.Patients.AnyAsync(x => x.CCCD == request.CCCD))
                throw new Exception("CCCD đã tồn tại");
                
            // Validation CCCD (12 số)
            if (!Regex.IsMatch(request.CCCD, @"^\d{12}$"))
                throw new Exception("CCCD phải có đúng 12 chữ số");
                
            // Validation Phone (10 số, bắt đầu bằng 0)
            if (!Regex.IsMatch(request.Phone, @"^0\d{9}$"))
                throw new Exception("Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số");
                
            // Validation EmergencyContact (10 số, bắt đầu bằng 0)
            if (!Regex.IsMatch(request.EmergencyContact, @"^0\d{9}$"))
                throw new Exception("Số điện thoại khẩn cấp phải bắt đầu bằng 0 và có 10 chữ số");

            //  Kiểm tra và chuẩn hóa tên
            request.FullName = request.FullName.Trim();
            if (string.IsNullOrEmpty(request.FullName))
                throw new Exception("Không được để trống tên");

            if (!Regex.IsMatch(request.FullName, @"^[a-zA-ZÀ-ỹĂăÂâĐđÊêÔôƠơƯư\s]+$"))
                throw new Exception("Tên không được chứa kí tự đặc biệt");
        
        //  Tạo User mới
        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var newUser = new User
        {
            Email = request.Email,
            Password = hashedPassword,
            Status = UserStatus.Active
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync(); // Phải save để lấy được newUser.Id

        //  Tạo mã bệnh nhân
        string patientCode;
        try
        {
            if (!string.IsNullOrEmpty(request.Code) && request.Code != "string")
            {
                patientCode = request.Code;
            }
            else
            {
                patientCode = await CheckUniqueCodeAsync();
            }

            // Kiểm tra code có bị trùng không
            int maxAttempts = 10;
            int attempts = 0;
            while (await _context.Patients.AnyAsync(p => p.Code == patientCode) && attempts < maxAttempts)
            {
                patientCode = await CheckUniqueCodeAsync();
                attempts++;
            }
            
            if (attempts >= maxAttempts)
            {
                throw new Exception("Không thể tạo mã bệnh nhân duy nhất sau nhiều lần thử");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating patient code: {ex.Message}");
            throw new Exception("Lỗi khi tạo mã bệnh nhân: " + ex.Message);
        }

        //  Gán thông tin Patient
        var patient = new Patient
        {
            Code = patientCode,
            Name = request.FullName,
            Phone = request.Phone,
            CCCD = request.CCCD,
            Address = request.Address,
            Gender = request.Gender,
            Dob = request.Dob,
            EmergencyContact = request.EmergencyContact,
            InsuranceNumber = "",
            Allergies = "",
            BloodType = "",
            Status = PatientStatus.Active,
            CreateDate = DateTime.Now.AddHours(7),
            UpdateDate = DateTime.Now.AddHours(7),
            CreateBy = request.FullName,
            UpdateBy = request.FullName,
            ImageURL = "",
            UserId = newUser.Id
        };
        await _context.Patients.AddAsync(patient);

        //  Gán role PATIENT (nếu dùng User_Role)
        var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Trim().ToLower() == "patient");
        if (patientRole != null)
        {
            await _context.User_Roles.AddAsync(new User_Role
            {
                UserId = newUser.Id,
                RoleId = patientRole.Id
            });
        }

        await _context.SaveChangesAsync();

        //  Trả về response
        var response = new PatientRegisterResponse
        {
            PatientId = patient.Id,
            FullName = patient.Name,
            Email = newUser.Email
        };
        
        Console.WriteLine($"=== AUTH SERVICE: Registration successful ===");
        Console.WriteLine($"PatientId: {response.PatientId}");
        Console.WriteLine($"FullName: {response.FullName}");
        Console.WriteLine($"Email: {response.Email}");
        
        return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== AUTH SERVICE: Registration failed ===");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, UserType userType)
    {
        try
        {
            // Tìm user có token hợp lệ
            var user = await _context.Users
                .Include(u => u.User_Roles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u =>
                    u.ResetPasswordToken == token &&
                    u.ResetPasswordTokenExpiryTime > DateTime.Now &&
                    u.User_Roles.Any(ur => ur.Role.Name.ToLower() == userType.ToString().ToLower())
                );

            if (user == null)
                throw new Exception("Token không hợp lệ hoặc đã hết hạn!");

            user.Password = _passwordHasher.HashPassword(newPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiryTime = null;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Đặt lại mật khẩu không thành công", ex);
        }
    }
}
