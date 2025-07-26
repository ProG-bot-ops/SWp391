namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Role;

public class RoleUpdate
{
    
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }

    public RoleUpdate()
    {
    }

    public RoleUpdate(string name, string code, DateTime createDate, DateTime? updateDate, string createBy, string? updateBy)
    {
        Name = name;
        Code = code;
        CreateDate = createDate;
        UpdateDate = updateDate;
        CreateBy = createBy;
        UpdateBy = updateBy;
    }
}
