using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class Nurse_AppointmentDTO
{
    public NurseAppointmentStatus Status { get; set; }
    public int Id { get; set; }
    public int NurseId { get; set; }
    public int AppointmentId { get; set; }
}
