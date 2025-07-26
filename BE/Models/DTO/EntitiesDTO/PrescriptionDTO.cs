using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class PrescriptionDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string? Note { get; set; }
    public PrescriptionStatus Status { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
