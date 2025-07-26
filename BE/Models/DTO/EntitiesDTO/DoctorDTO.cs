using System.ComponentModel.DataAnnotations;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class DoctorDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    public DateTime Dob { get; set; }
    public string CCCD { get; set; }
    public string Phone { get; set; }
    public string? ImageURL { get; set; }
    public string LicenseNumber { get; set; }
    public float YearOfExperience { get; set; }
    public float WorkingHours { get; set; }
    [EnumDataType(typeof(DoctorStatus))]
    public DoctorStatus Status { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
