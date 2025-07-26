using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImportDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicineImportDetailMapper : IMedicineImportDetailMapper
    {

        public MedicineImportDetail CreateToEntity(MedicineImportDetailCreate create)
        {
            MedicineImportDetail entity = new MedicineImportDetail();
            entity.Id = create.Id;
            entity.ImportId = create.ImportId;
            entity.MedicineId = create.MedicineId;
            entity.BatchNumber = create.BatchNumber;
            entity.Quantity = create.Quantity;
            entity.UnitPrice = create.UnitPrice;
            entity.ManufactureDate = create.ManufactureDate;
            entity.ExpiryDate = create.ExpiryDate;
            entity.UnitId = create.UnitId;
    

            return entity;
        }

        public MedicineImportDetailResponseDTO EntityToResponse(MedicineImportDetail entity)
        {
            return new MedicineImportDetailResponseDTO
            {
                Id = entity.Id,
                MedicineName = entity.Medicine?.Name ?? "N/A",
                Quantity = entity.Quantity,
                UnitName = entity.Unit?.Name ?? "N/A",              
                UnitPrice = entity.UnitPrice,
                CategoryName = entity.Medicine?.MedicineCategory?.Name ?? "N/A",
                ExpiryDate = entity.ExpiryDate,
                ManufactureDate = entity.ManufactureDate,
                BatchNumber = entity.BatchNumber,              
                SupplierName = entity.Supplier?.Name ?? "N/A",              
                CreateDate = entity.CreateDate,
                CreateBy = entity.CreateBy
            };


        }
 

        public IEnumerable<MedicineImportDetailResponseDTO> ListEntityToResponse(IEnumerable<MedicineImportDetail> entities)
        {
            return entities.Select(x => EntityToResponse(x)).ToList();
        }

        public MedicineImportDetail UpdateToEntity(MedicineImportDetailUpdate update)
        {
            MedicineImportDetail response = new MedicineImportDetail();
            response.Id = update.Id;
            response.ImportId = update.ImportId;
            response.MedicineId = update.MedicineId;
            response.BatchNumber = update.BatchNumber;
            response.Quantity = update.Quantity;
            response.UnitPrice = update.UnitPrice;
            response.ManufactureDate= update.ManufactureDate;
            response.ExpiryDate = update.ExpiryDate;
            response.UnitId = update.UnitId;

            return response;
        }
    }
}
