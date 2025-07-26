using static SWP391_SE1914_ManageHospital.Ultility.Status;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class DepartmentResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int TotalAmountOfPeople { get; set; }
    [EnumDataType(typeof(DepartmentStatus))]
    public DepartmentStatus Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }

    public DepartmentResponseDTO()
    {
    }

    public DepartmentResponseDTO(int id, string name, string code, string description, int totalAmountOfPeople, DepartmentStatus status, DateTime createDate, DateTime? updateDate, string createBy, string? updateBy)
    {
        Id = id;
        Name = name;
        Code = code;
        Description = description;
        TotalAmountOfPeople = totalAmountOfPeople;
        Status = status;
        CreateDate = createDate;
        UpdateDate = updateDate;
        CreateBy = createBy;
        UpdateBy = updateBy;
    }
}
