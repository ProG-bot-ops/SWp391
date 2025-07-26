using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authenication;

public class LoginRequestDTO
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Loại người dùng không được để trống")]
    public UserType UserType { get; set; }

}
public enum UserType
{
    Admin, Patient, Doctor, Nurse
}
