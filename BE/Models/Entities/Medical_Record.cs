using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Medical_Record : BaseEntity
{
    public MedicalRecordStatus Status { get; set; }
    public string Diagnosis { get; set; }
    public string TestResults { get; set; }
    public string? Notes { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int? PrescriptionId { get; set; } // Cho phép null
    public int? DiseaseId { get; set; } // Cho phép null
    public virtual Patient? Patient { get; set; }
    public virtual Doctor? Doctor { get; set; }
    public virtual Prescription? Prescription { get; set; }
    public virtual Disease? Disease { get; set; }
    public virtual Appointment? Appointment { get; set; }
}
