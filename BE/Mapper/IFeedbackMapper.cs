using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Feedback;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper;

public interface IFeedbackMapper
{
    // request => Entity(DTO)

    Feedback CreateToEntity(FeedbackCreate create);
    
    Feedback DeleteToEntity(FeedbackDelete delete);

    // Entity(DTO) => Response
    FeedbackResponseDTO EntityToResponse(Feedback entity);
    IEnumerable<FeedbackResponseDTO> ListEntityToResponse(IEnumerable<Feedback> entities);
}
