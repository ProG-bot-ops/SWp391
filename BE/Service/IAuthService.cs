using Microsoft.AspNetCore.Identity.Data;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authenication;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequestDTO request);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
    Task<PatientRegisterResponse> RegisterPatientAsync(PatientRegisterRequest request);
    

    Task<bool> LogoutAsync(int userId, UserType userType);
    Task<bool> ChangePasswordAsync(int userId, UserType userType, string oldPassword, string newPassword, string confirmPassword);
    Task<bool> ForgotPasswordAsync(string email, UserType userType);
    Task<bool> ResetPasswordAsync(string token, string newPassword, UserType userType);
}
