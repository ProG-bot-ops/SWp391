using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImportDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineImportDetailMapper
    {
        MedicineImportDetail CreateToEntity(MedicineImportDetailCreate create);
        MedicineImportDetail UpdateToEntity(MedicineImportDetailUpdate update);

        MedicineImportDetailResponseDTO EntityToResponse(MedicineImportDetail entity);
        IEnumerable<MedicineImportDetailResponseDTO> ListEntityToResponse(IEnumerable<MedicineImportDetail> entities);
    }
}
