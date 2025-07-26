using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineManageForAdminMapper
    {
        MedicineManageForAdminDTO EntityToInventoryDTO(Medicine entity);
        IEnumerable<MedicineManageForAdminDTO> ListEntityToInventoryDTO(IEnumerable<Medicine> entities);
    }
}
