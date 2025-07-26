using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class AppointmentDTO
{

    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    [EnumDataType(typeof(AppointmentStatus))]
    public AppointmentStatus Status { get; set; }
    public string? Note { get; set; }
    public bool isSend { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public int PatientId { get; set; }
    public int ClinicId { get; set; }
    public int ReceptionId { get; set; }
    
}
