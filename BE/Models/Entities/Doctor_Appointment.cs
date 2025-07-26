using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Doctor_Appointment
{
    public int Id { get; set; }
    public DoctorAppointmentStatus Status { get; set; }
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public virtual Appointment Appointment { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
}
