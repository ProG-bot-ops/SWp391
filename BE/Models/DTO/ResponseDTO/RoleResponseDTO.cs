namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class RoleResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }

    public RoleResponseDTO()
    {
    }

    public RoleResponseDTO(int id, string name, string code, DateTime createDate, DateTime? updateDate, string createBy, string? updateBy)
    {
        Id = id;
        Name = name;
        Code = code;
        CreateDate = createDate;
        UpdateDate = updateDate;
        CreateBy = createBy;
        UpdateBy = updateBy;
    }
}
