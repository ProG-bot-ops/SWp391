using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Doctor : BaseEntity
{
    public Gender Gender { get; set; }
    public DateTime Dob { get; set; }
    public string CCCD { get; set; }
    public string Phone { get; set; }
    public string? ImageURL { get; set; }
    public string LicenseNumber { get; set; }
    public float YearOfExperience { get; set; }
    public float WorkingHours { get; set; }
    public DoctorStatus Status { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public int? ClinicId { get; set; }
    public virtual User User { get; set; }
    public virtual Department Department { get; set; }
    public virtual Clinic? Clinic { get; set; }
    public virtual ICollection<Doctor_Appointment> Doctor_Appointments { get; set; } = new List<Doctor_Appointment>();
    public virtual ICollection<Medical_Record> Medical_Records { get; set; } = new List<Medical_Record>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<Doctor_Shift> Doctor_Shifts { get; set; } = new List<Doctor_Shift>();


}
