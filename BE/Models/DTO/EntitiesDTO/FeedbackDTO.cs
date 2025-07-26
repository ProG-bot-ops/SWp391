namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class FeedbackDTO
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public int PatientId { get; set; }
    public int? DoctorId { get; set; }
    public int? AppointmentId { get; set; }
}
