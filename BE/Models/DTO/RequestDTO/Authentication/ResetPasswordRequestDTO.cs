using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authenication;

public class ResetPasswordRequestDTO
{
    [Required]
    public string Token { get; set; }

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; }

    [Required]
    public UserType UserType { get; set; }
}
