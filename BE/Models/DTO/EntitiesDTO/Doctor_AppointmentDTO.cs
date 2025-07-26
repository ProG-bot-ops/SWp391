using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class Doctor_AppointmentDTO
{
    public int Id { get; set; }
    public DoctorAppointmentStatus Status { get; set; }
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
}
