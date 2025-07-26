using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Nurse_Appointment
{
    public NurseAppointmentStatus Status { get; set; }
    public int Id { get; set; }
    public int NurseId { get; set; }
    public int AppointmentId { get; set; }
    public virtual Nurse Nurse { get; set; } = null!;
    public virtual Appointment Appointment { get; set; } = null!;
}
