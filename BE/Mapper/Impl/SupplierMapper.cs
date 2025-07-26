using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Supplier;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class SupplierMapper : ISupplierMapper
    {
        public Supplier CreateToEntity(SupplierCreate create)
        {
            Supplier supplier = new Supplier();
            supplier.Name = create.Name;
            supplier.Phone = create.Phone;
            supplier.Email = create.Email;
            supplier.Address = create.Address;
            supplier.Code = create.Code;
            return supplier;
        }

        public SupplierResponeDTO EntityToResponse(Supplier entity)
        {
            SupplierResponeDTO respone = new SupplierResponeDTO();
            respone.Id = entity.Id;
            respone.Name = entity.Name;
            respone.Phone = entity.Phone;
            respone.Email = entity.Email;
            respone.Address = entity.Address;
            respone.Code = entity.Code;
            respone.CreateDate = entity.CreateDate;
            respone.CreateBy = entity.CreateBy;
            return respone;
        }

        public IEnumerable<SupplierResponeDTO> ListEntityToResponse(IEnumerable<Supplier> entities)
        {
            return entities.Select(x => EntityToResponse(x)).ToList();
        }

        public Supplier UpdateToEntity(SupplierUpdate update)
        {
            Supplier supplier = new Supplier();
            supplier.Name = update.Name;
            supplier.Phone = update.Phone;
            supplier.Email = update.Email;
            supplier.Address = update.Address;
            supplier.UpdateDate = update.UpdateDate;
            supplier.UpdateBy = update.UpdateBy;
            return supplier;
        }
    }
}
