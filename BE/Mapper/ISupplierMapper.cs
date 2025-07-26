using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Supplier;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface ISupplierMapper
    {
        Supplier CreateToEntity(SupplierCreate create);
        Supplier UpdateToEntity(SupplierUpdate update);

        SupplierResponeDTO EntityToResponse(Supplier entity);
        IEnumerable<SupplierResponeDTO> ListEntityToResponse(IEnumerable<Supplier> entities);
    }
}
