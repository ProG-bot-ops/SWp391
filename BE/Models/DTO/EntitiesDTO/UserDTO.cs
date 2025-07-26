using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class UserDTO
{
    public int Id;
    public string Email { get; set; }
    public string Password { get; set; }
    public UserStatus Status { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiryTime { get; set; }
}
