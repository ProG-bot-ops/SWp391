using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IMedicineInventoryService
    {
        Task<MedicineInventoryPageDTO> GetPagedAsync(int pageNumber, int pageSize);
        Task<MedicineInventoryPageDTO> SearchAsync(string keyword, string sortBy, bool ascending, int pageNumber, int pageSize = 10);

    }
}
