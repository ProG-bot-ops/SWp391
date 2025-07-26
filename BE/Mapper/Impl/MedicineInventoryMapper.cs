using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicineInventoryMapper : IMedicineInventoryMapper
    {
        public MedicineInventoryResponseDTO EntityToResponse(Medicine_Inventory entity)
        {
            if(entity == null || entity.ImportDetail == null)
            {
                return new MedicineInventoryResponseDTO();
            }

            var detail = entity.ImportDetail;
            var medicine = detail.Medicine;
            var supplier = detail.Supplier;
            var unit = detail.Unit;
            var category = medicine.MedicineCategory;

            return new MedicineInventoryResponseDTO
            {
                Id = entity.Id,
                MedicineCode = medicine?.Code ?? string.Empty,
                BatchNumber = detail.BatchNumber ?? string.Empty,
                MedicineName = medicine?.Name ?? string.Empty,
                CategoryName = category?.Name ?? string.Empty,
                UnitName = unit?.Name ?? string.Empty,
                Quantity = entity.Quantity,
                Status = entity.Status,
                ManufactureDate = detail.ManufactureDate,
                ExpiryDate = detail.ExpiryDate,
                SupplierName = supplier?.Name ?? string.Empty,
                ImportDate = detail.CreateDate,
            };
        }

        public IEnumerable<MedicineInventoryResponseDTO> ListEntityToResponse(IEnumerable<Medicine_Inventory> entities)
        {
            return entities.Select(EntityToResponse);
        }
    }
}
