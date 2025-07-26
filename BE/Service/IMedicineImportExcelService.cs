using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ImportMedicineEX;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IMedicineImportExcelService
    {
        Task<MedicineImportRequest> ParseImportExcelToRequest(IFormFile file, int supplierId);
        Task<bool> ConfirmImportAsync(MedicineImportRequest request);
    }
}