namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Authenication;

public class LoginResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpiration { get; set; }
    public UserInfo UserInfo { get; set; }
    public string RedirectUrl { get; set; } // Sẽ là "/dashboard" cho Admin,... và "/home" cho Patient

}
public class UserInfo
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public UserType UserType { get; set; }
    public ICollection<string> Roles { get; set; } 
}
