using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest;

namespace SWP391_SE1914_ManageHospital.Models.Services
{
    public interface IShiftRequestService
    {
        Task<ShiftRequestResponseDTO> CreateAsync(ShiftRequestRequestDTO dto);
        Task<IEnumerable<ShiftRequestResponseDTO>> GetByDoctorAsync(int doctorId);
        Task<IEnumerable<ShiftRequestResponseDTO>> GetAllAsync();
        Task<bool> ApproveAsync(int requestId);
        Task<bool> RejectAsync(int requestId, string? reason = null);
    }
}