using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Patient : BaseEntity
{
    public Gender Gender { get; set; }
    public DateTime Dob { get; set; }
    public string CCCD { get; set; }
    public string Phone { get; set; }
    public string? EmergencyContact { get; set; }
    public string Address { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? Allergies { get; set; }
    public PatientStatus Status { get; set; }
    public string? BloodType { get; set; }
    public string? ImageURL { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
    public virtual ICollection<Medical_Record> Medical_Records { get; set; } = new List<Medical_Record>();
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    



}
