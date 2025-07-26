using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineCategoryMapper
    {
        MedicineCategoryResponseDTO MapToDTO(MedicineCategory entity);
        MedicineCategory MapToEntity(MedicineCategoryRequest request);
        void MapToExistingEntity(MedicineCategoryRequest request, MedicineCategory entity);
    }
}
