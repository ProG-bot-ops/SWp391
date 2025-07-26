using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineDetailMapper
    {
        MedicineDetailResponseDTO MapToResponse(MedicineDetail entity);
        MedicineDetail MapToEntity(MedicineDetailRequest request);
        void MapToExistingEntity(MedicineDetailRequest request, MedicineDetail entity);
    }
}
