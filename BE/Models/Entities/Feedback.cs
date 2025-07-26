namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Feedback
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public int PatientId { get; set; }
    public int? DoctorId { get; set; }
    public int? AppointmentId { get; set; }
    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual Appointment Appointment { get; set; }
    
}
