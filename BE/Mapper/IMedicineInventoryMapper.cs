using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineInventoryMapper
    {
        MedicineInventoryResponseDTO EntityToResponse(Medicine_Inventory entity);
        IEnumerable<MedicineInventoryResponseDTO> ListEntityToResponse(IEnumerable<Medicine_Inventory> entities);
    }
}
