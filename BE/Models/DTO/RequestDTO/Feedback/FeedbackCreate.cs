namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Feedback;

public class FeedbackCreate
{
    public string Content { get; set; }
    public int? DoctorId { get; set; }
    public int? AppointmentId { get; set; }

    public FeedbackCreate()
    {
    }

    public FeedbackCreate(string content, int? doctorId, int? appointmentId)
    {
        Content = content;
        DoctorId = doctorId;
        AppointmentId = appointmentId;
    }
}
