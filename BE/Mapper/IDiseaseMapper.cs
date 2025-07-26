using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Disease;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IDiseaseMapper
    {
        DiseaseResponse MapToResponse(DiseaseDTO entity);
        DiseaseDTO MapToEntity(DiseaseCreateRequest request);
        DiseaseDTO MapToEntity(DiseaseUpdateRequest request, DiseaseDTO existingEntity);
    }
}