using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicineImportMapper : IMedicineImportMapper
    {
        public MedicineImport CreateToEntity(MedicineImportCreate create)
        {
            MedicineImport medicineImport = new MedicineImport();
            medicineImport.Name = create.Name;
            medicineImport.Notes = create.Notes;
            medicineImport.SupplierId = create.SupplierId;

            return medicineImport;
        }

        public MedicineImportResponseDTO EntityToResponse(MedicineImport entity)
        {
            MedicineImportResponseDTO response = new MedicineImportResponseDTO();
            response.Id = entity.Id;
            response.Name = entity.Name;
            response.Notes = entity.Notes;
            response.SupplierId = entity.SupplierId;

            return response;
        }

        public IEnumerable<MedicineImportResponseDTO> ListEntityToResponse(IEnumerable<MedicineImport> entities)
        {
            return entities.Select(x => EntityToResponse(x)).ToList();
        }

        public MedicineImport UpdateToEntity(MedicineImportUpdate update)
        {
            MedicineImport entity = new MedicineImport();
            entity.Id = update.Id;
            entity.Name = update.Name;
            entity.Notes = update.Notes;
            entity.SupplierId = update.SupplierId;
            entity.CreateDate = update.CreateDate;
            entity.CreateBy = update.CreateBy;
            entity.UpdateDate = update.UpdateDate;
            entity.UpdateBy = update.UpdateBy;

            return entity;
        }
    }
}
