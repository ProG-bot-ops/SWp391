using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineImportMapper
    {
        MedicineImport CreateToEntity(MedicineImportCreate create); 
        MedicineImport UpdateToEntity(MedicineImportUpdate update);
        MedicineImportResponseDTO EntityToResponse(MedicineImport entity);
        IEnumerable<MedicineImportResponseDTO> ListEntityToResponse(IEnumerable<MedicineImport> entities);
    }
}
