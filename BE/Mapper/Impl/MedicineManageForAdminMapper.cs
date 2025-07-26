using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicineManageForAdminMapper : IMedicineManageForAdminMapper
    {
        public MedicineManageForAdminDTO EntityToInventoryDTO(Medicine entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MedicineManageForAdminDTO> ListEntityToInventoryDTO(IEnumerable<Medicine> entities)
        {
            List<MedicineManageForAdminDTO> response = new List<MedicineManageForAdminDTO>();
            foreach (var entity in entities)
            {
                var latestDetail = entity.MedicineImportDetails?
                        .OrderByDescending(i => i.Import.CreateDate)
                        .FirstOrDefault();
                var medicineDetail = entity.MedicineDetail;
                var dto = new MedicineManageForAdminDTO
                {
                    MedicineId = entity.Id,
                    MedicineName = entity.Name,
                    CategoryName = entity.MedicineCategory?.Name ?? string.Empty,
                    UnitName = entity.Unit?.Name ?? string.Empty,
                    LatestUnitPrice = latestDetail?.UnitPrice ?? 0,
                    TotalQuantityInStock = entity.MedicineImportDetails?.Sum(i => i.Quantity) ?? 0,
                    BatchNumber = latestDetail?.BatchNumber ?? "",
                    ImportDate = latestDetail?.Import?.CreateDate ?? DateTime.MinValue,
                    ExpiryDate = latestDetail?.ExpiryDate ?? DateTime.MinValue,
                    SupplierName = latestDetail?.Import?.Supplier?.Name ?? string.Empty,
                    Ingredients = medicineDetail?.Ingredients ?? string.Empty
                };

                response.Add(dto);
            }

            return response;
        }
    }
}
