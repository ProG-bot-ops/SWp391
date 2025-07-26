namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Feedback;

public class FeedbackDelete
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public int PatientId { get; set; }
    public int? DoctorId { get; set; }
    public int? AppointmentId { get; set; }

    public FeedbackDelete()
    {
    }

    public FeedbackDelete(int id, string content, DateTime createDate, int patientId, int? doctorId, int? appointmentId)
    {
        Id = id;
        Content = content;
        CreateDate = createDate;
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentId = appointmentId;
    }
}
