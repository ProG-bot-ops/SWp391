namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class User_Role
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}
