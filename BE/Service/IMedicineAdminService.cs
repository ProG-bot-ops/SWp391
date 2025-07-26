using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineAdmin;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IMedicineAdminService
    {
        Task<MedicineAdminPage> GetMedicineAsync(int pageNumber, int pageSize);
        Task<MedicineAdminPage> Search(string keyword ,decimal? startPrice, decimal? endPrice, int pageNumber, int pageSize);
        Task<bool> UpdateMedicineAsync(int id, MedicineAdminUpdate update);
        Task<bool> UpdateMedicineImageAsync(int id, string imageUrl);
    }
}
