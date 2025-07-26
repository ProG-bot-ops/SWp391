using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authenication;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authentication;

public class ForgotPasswordRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public UserType UserType { get; set; }
}
