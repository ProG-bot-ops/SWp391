using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserStatus Status { get; set; }
    public string? RefreshToken { get; set; } 
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiryTime { get; set; } 
    public virtual ICollection<User_Role> User_Roles { get; set; } = new List<User_Role>();
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    public virtual ICollection<Nurse> Nurses { get; set; } = new List<Nurse>();
    public virtual ICollection<Reception> Receptions { get; set; } = new List<Reception>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
