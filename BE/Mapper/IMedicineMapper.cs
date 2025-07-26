using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineMapper
    {
        MedicineResponseDTO MapToDTO(Medicine entity);
        Medicine MapToEntity(MedicineRequest request);
        void MapToExistingEntity(MedicineRequest request, Medicine entity);

    }
}

