using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authentication;

public class ChangePasswordRequestDTO
{
    [Required]
    public string OldPassword { get; set; }

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
