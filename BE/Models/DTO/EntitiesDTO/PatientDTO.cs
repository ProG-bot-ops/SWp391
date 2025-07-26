using System.ComponentModel.DataAnnotations;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class PatientDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string Code { get; set; }

    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    public DateTime Dob { get; set; }
    public string CCCD { get; set; }
    public string Phone { get; set; }
    public string? EmergencyContact { get; set; }
    public string Address { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? Allergies { get; set; }

    [EnumDataType(typeof(PatientStatus))]
    public PatientStatus Status { get; set; }
    public string? BloodType { get; set; }
    public string? ImageURL { get; set; }
    public int UserId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
