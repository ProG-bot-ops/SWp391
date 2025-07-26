using static SWP391_SE1914_ManageHospital.Ultility.Status;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;

public class ClinicCreate
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    [EnumDataType(typeof(ClinicStatus))]
    public ClinicStatus Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; } = string.Empty;
    public string? UpdateBy { get; set; }

    public ClinicCreate()
    {
    }

    public ClinicCreate(string name, string code, ClinicStatus status, DateTime createDate, DateTime? updateDate, string createBy, string? updateBy)
    {
        Name = name;
        Code = code;
        Status = status;
        CreateDate = createDate;
        UpdateDate = updateDate;
        CreateBy = createBy;
        UpdateBy = updateBy;
    }
}
