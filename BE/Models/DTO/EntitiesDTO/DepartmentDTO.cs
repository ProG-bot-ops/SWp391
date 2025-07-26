using System.ComponentModel.DataAnnotations;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class DepartmentDTO
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
}
