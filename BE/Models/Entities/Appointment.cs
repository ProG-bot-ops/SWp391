using System.ComponentModel.DataAnnotations;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Appointment : BaseEntity
{

    public DateTime AppointmentDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Shift { get; set; } // "morning" hoặc "afternoon"

    [EnumDataType(typeof(AppointmentStatus))]
    public AppointmentStatus Status { get; set; }
    public string? Note { get; set; }
    public bool isSend { get; set; }
    public int PatientId { get; set; }
    public int ClinicId { get; set; }
    public int? ReceptionId { get; set; }
    public int? ServiceId { get; set; } // Thêm dòng này để lưu dịch vụ bác sĩ
    public virtual Patient? Patient { get; set; }
    public virtual Clinic? Clinic { get; set; }
    public virtual Invoice? Invoice { get; set; }
    public virtual Reception? Reception { get; set; }
    public virtual Medical_Record? Medical_Record { get; set; }
    public virtual Service? Service { get; set; } // Navigation property
    public virtual ICollection<Doctor_Appointment> Doctor_Appointments { get; set; } = new List<Doctor_Appointment>();
    public virtual ICollection<Nurse_Appointment> Nurse_Appointments { get; set; } = new List<Nurse_Appointment>();
    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    
}
