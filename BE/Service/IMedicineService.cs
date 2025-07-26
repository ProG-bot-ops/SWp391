using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public interface IMedicineService
{
    Task<List<MedicineResponseDTO>> GetAllAsync();
    Task<MedicineResponseDTO?> GetByIdAsync(int id);
    Task<MedicineResponseDTO> CreateAsync(MedicineRequest request);
    Task<MedicineResponseDTO?> UpdateAsync(int id, MedicineRequest request);
    Task<bool> DeleteAsync(int id);
    Task<List<MedicineResponseDTO>> GetByPrescribedAsync(int prescribed);

    Task<IEnumerable<MedicineResponseDTO>> GetByCategoryIdAsync(int categoryId);

}
