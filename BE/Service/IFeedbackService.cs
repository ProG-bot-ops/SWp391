using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Feedback;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IFeedbackService
{
     Task<IEnumerable<FeedbackResponseDTO>> GetAllFeedbackAsync();
     Task<IEnumerable<FeedbackResponseDTO>> SearchFeedbackByFilterAsync(string? name, DateTime? appointmentDate, DateTime? startTime);
     Task<FeedbackResponseDTO> CreateFeedbackAsync(FeedbackCreate create);
     
     Task<bool> HardDeleteFeedbackAsync(int id);
     Task<IEnumerable<FeedbackResponseDTO>> FindFeedbackByPatientIdAsync(int id);
     
}
