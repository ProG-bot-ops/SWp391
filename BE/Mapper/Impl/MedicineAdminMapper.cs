using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;


namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicineAdminMapper : IMedicineAdminMapper
    {
        public MedicineAdmin EntityToResponse(Medicine entity)
        {
            if (entity == null || entity.Unit == null || entity.MedicineCategory == null || entity.MedicineDetail == null)
            {
                return new MedicineAdmin();
            }
                
            var md = entity.MedicineDetail;
            var un = entity.Unit;
            var mc = entity.MedicineCategory;

           


            return new MedicineAdmin
            {
                    Id = entity.Id,
                    MedicineImage = entity.ImageUrl ?? string.Empty,
                    MedicineCode = entity.Code,
                    MedicineName = entity.Name,
                    MedicineStatus = entity.Status,
                    MDescription = entity.Description ?? string.Empty,
                    MDDescription = md?.Description ?? string.Empty,
                    Prescribled = entity.Prescribed,
                    UnitName = un?.Name ?? string.Empty,
                    UnitPrice = entity.UnitPrice,
                    CategoryName = mc?.Name ?? string.Empty,
                    Ingredients = md?.Ingredients ?? string.Empty,
                    Warning = md?.Warning ?? string.Empty,
                    StorageIntructions = md?.StorageInstructions ?? string.Empty,
            };
                
        }
        

        public IEnumerable<MedicineAdmin> ListEntityToResponse(IEnumerable<Medicine> entities)
        {
            return entities.Select(EntityToResponse);
        }
    }
}
