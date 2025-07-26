using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Disease;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using System.Collections.Generic;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IDiseaseService
    {
        IEnumerable<DiseaseResponse> GetAllDiseases();
        DiseaseResponse GetDiseaseDetail(int id);
        DiseaseResponse CreateDisease(DiseaseCreateRequest request);
        DiseaseResponse UpdateDisease(int id, DiseaseUpdateRequest request);
        bool DeleteDisease(int id);
    }
}