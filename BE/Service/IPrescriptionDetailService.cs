// Service/IPrescriptionDetailService.cs
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PrescriptionDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IPrescriptionDetailService
    {
        Task<List<PrescriptionDetailResponseDTO>> GetAllAsync();
        Task<PrescriptionDetailResponseDTO?> GetByIdAsync(int id);
        Task<List<PrescriptionDetailResponseDTO>> GetByPrescriptionIdAsync(int prescriptionId);
        // Tạo mới chỉ cần request (có chứa req.UserId)
        Task<PrescriptionDetailResponseDTO> CreateAsync(
        PrescriptionDetailRequest req,
        int userId
    );

        // Cập nhật chỉ cần id + request


        Task<IEnumerable<PrescriptionDetailResponseDTO>> GetByUserAsync(int userId, string role);
        Task<bool> DeleteAsync(int id);
    }
}
