using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ImportMedicineEX;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImportDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;


namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicineImportExcelMapper : IMedicineImportExcelMapper
    {
        public MedicineImportDetail MapToImportDetailEntity(MedicineImportDetailRequest request, int medicineId, int importId)
        {
            return new MedicineImportDetail
            {
                MedicineId = medicineId,
                ImportId = importId,
                BatchNumber = request.BatchNumber,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                ManufactureDate = request.ManufactureDate,
                ExpiryDate = request.ExpiryDate,
                Name = request.MedicineName,
                Code = request.MedicineCode,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                CreateBy = "system",
                UpdateBy = "system"
            };
        }

        public Medicine_Inventory MapToInventoryEntity(MedicineImportDetailRequest request, int medicineId, int importDetailId)
        {
            return new Medicine_Inventory
            {
                MedicineId = medicineId,
                ImportDetailId = importDetailId,
                BatchNumber = request.BatchNumber,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                ImportDate = DateTime.UtcNow,
                ExpiryDate = request.ExpiryDate,
                Status = MedicineInventoryStatus.InStock,
            };
        }

        public MedicineImport MapToImportEntity(MedicineImportRequest request, int supplierId)
        {
            return new MedicineImport
            {
                Code = request.ImportCode,
                Name = request.ImportName,
                SupplierId = supplierId,
                Notes = request.Notes,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                CreateBy = "system",
                UpdateBy = "system"
            };
        }
    }
}