namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Role : BaseEntity
{
    
    public virtual ICollection<User_Role> User_Roles { get; set; } = new List<User_Role>();

}
