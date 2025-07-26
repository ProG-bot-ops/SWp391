using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IMedicineCategoryService
    {
        Task<List<MedicineCategoryResponseDTO>> GetAllAsync();
        Task<MedicineCategoryResponseDTO?> GetByIdAsync(int id);
        Task<MedicineCategoryResponseDTO> CreateAsync(MedicineCategoryRequest request);
        Task<MedicineCategoryResponseDTO?> UpdateAsync(int id, MedicineCategoryRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
