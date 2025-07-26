using static SWP391_SE1914_ManageHospital.Ultility.Status;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;

public class ClinicUpdate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    [EnumDataType(typeof(ClinicStatus))]
    public ClinicStatus Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public ClinicUpdate()
    {
    }

    public ClinicUpdate(int id, string name, string code, ClinicStatus status, DateTime createDate, DateTime? updateDate, string createBy, string? updateBy)
    {
        Id = id;
        Name = name;
        Code = code;
        Status = status;
        CreateDate = createDate;
        UpdateDate = updateDate;
        CreateBy = createBy;
        UpdateBy = updateBy;
    }
}
