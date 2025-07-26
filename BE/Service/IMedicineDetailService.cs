using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IMedicineDetailService
    {
        Task<List<MedicineDetailResponseDTO>> GetAllAsync();
        Task<MedicineDetailResponseDTO> GetByIdAsync(int id);
        Task<MedicineDetailResponseDTO> CreateAsync(MedicineDetailRequest request);
        Task<MedicineDetailResponseDTO> UpdateAsync(int id, MedicineDetailRequest request);
        Task<bool> DeleteAsync(int id);

        // Thêm phương thức tìm kiếm theo thành phần
        Task<List<MedicineDetailResponseDTO>> SearchByIngredientsAsync(string ingredients);
    }

}
