using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Feedback;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl;

public class FeedbackMapper : IFeedbackMapper
{
    public Feedback CreateToEntity(FeedbackCreate create)
    {
        Feedback feedback = new Feedback();
        feedback.Content = create.Content;
        return feedback;
    }

    public Feedback DeleteToEntity(FeedbackDelete delete)
    {
        Feedback feedback = new Feedback();
        feedback.Id = delete.Id;
        feedback.Content = delete.Content;
        feedback.DoctorId = delete.DoctorId;
        feedback.AppointmentId = delete.AppointmentId;
        return feedback;
    }

    public FeedbackResponseDTO EntityToResponse(Feedback entity)
    {
        FeedbackResponseDTO response = new FeedbackResponseDTO();
        response.Id = entity.Id;
        response.Content = entity.Content;
        //response.AppointmentId = entity.AppointmentId;
        //response.DoctorId = entity.DoctorId;
        //response.PatientId = entity.PatientId;
        response.CreateDate = entity.CreateDate;
        return response;
    }

    public IEnumerable<FeedbackResponseDTO> ListEntityToResponse(IEnumerable<Feedback> entities)
    {
        return entities.Select(x => EntityToResponse(x)).ToList();
    }
}
