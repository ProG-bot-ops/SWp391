using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Prescription : BaseEntity
{
    public string? Note { get; set; }
    public PrescriptionStatus Status { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual ICollection<Medical_Record> Medical_Records { get; set; } = new List<Medical_Record>();
    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}
