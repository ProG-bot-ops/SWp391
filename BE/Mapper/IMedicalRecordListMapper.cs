using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicalRecordListMapper
    {
        MedicalRecordResponse EntityToListResponse(Medical_Record entity);
        IEnumerable<MedicalRecordResponse> ListEntityToResponse(IEnumerable<Medical_Record> entities);
    }
}