using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Supplier;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface ISupplierService
    {
        public Task<IEnumerable<SupplierResponeDTO>> GetAllSupplierAsync();
        public Task<IEnumerable<SupplierResponeDTO>> SearchSupplierByKeyAsync(string name);
        public Task<SupplierResponeDTO> UpdateSupplerAsync(int id, SupplierUpdate update);
        public Task<SupplierResponeDTO> CreateSupplerAsync(SupplierCreate create);
        public Task<string> CheckUniqueCodeAsync();
    }
}
