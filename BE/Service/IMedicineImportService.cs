using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IMedicineImportService
    {
        public Task<IEnumerable<MedicineImportResponseDTO>> GetAllMedicineImportAsync();
        public Task<IEnumerable<MedicineImportResponseDTO>> SearchMedicineImportByName(string name);
        public Task<MedicineImportResponseDTO> UpdateMedicineImportAsync(int id, MedicineImportUpdate update);
        public Task<MedicineImportResponseDTO> CreateMedicineImportAsync(MedicineImportCreate create);
        public Task<string> CheckUniqueCodeAsync();
    }
}
