using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ImportMedicineEX;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IMedicineImportExcelMapper
    {
        MedicineImport MapToImportEntity(MedicineImportRequest request, int supplierId);
        MedicineImportDetail MapToImportDetailEntity(MedicineImportDetailRequest request, int medicineId, int importId);
        Medicine_Inventory MapToInventoryEntity(MedicineImportDetailRequest request, int medicineId, int importDetailId);
    }
}