using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class Medical_RecordDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public MedicalRecordStatus Status { get; set; }
    public string Diagnosis { get; set; }
    public string TestResults { get; set; }
    public string? Notes { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int? PrescriptionId { get; set; }
    public int? DiseaseId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
