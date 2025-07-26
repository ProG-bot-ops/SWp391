using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicineDetailMapper : IMedicineDetailMapper
    {
        public MedicineDetailResponseDTO MapToResponse(MedicineDetail entity)
        {
            return new MedicineDetailResponseDTO
            {
                Id = entity.Id,
                MedicineId = entity.MedicineId,
                Ingredients = entity.Ingredients,
                ExpiryDate = entity.ExpiryDate,
                Manufacturer = entity.Manufacturer,
                Warning = entity.Warning,
                StorageInstructions = entity.StorageInstructions,
                Status = (MedicineStatus)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CreateBy = entity.CreateBy,
                UpdateBy = entity.UpdateBy,
                Description = entity.Description
            };
        }

        public MedicineDetail MapToEntity(MedicineDetailRequest request)
        {
            return new MedicineDetail
            {
                MedicineId = request.MedicineId,
                Ingredients = request.Ingredients,
                ExpiryDate = request.ExpiryDate,
                Manufacturer = request.Manufacturer,
                Warning = request.Warning,
                StorageInstructions = request.StorageInstructions,
                Status = (int)(MedicineStatus)request.Status,
                Description = request.Description,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                CreateBy = "system", // Giả sử giá trị mặc định
                UpdateBy = "system"  // Giả sử giá trị mặc định
            };
        }

        public void MapToExistingEntity(MedicineDetailRequest request, MedicineDetail entity)
        {
            entity.MedicineId = request.MedicineId;
            entity.Ingredients = request.Ingredients;
            entity.ExpiryDate = request.ExpiryDate;
            entity.Manufacturer = request.Manufacturer;
            entity.Warning = request.Warning;
            entity.StorageInstructions = request.StorageInstructions;
            entity.Status = request.Status;
            entity.Description = request.Description;
            entity.UpdateDate = DateTime.UtcNow;
            entity.UpdateBy = "system"; // Giả sử giá trị mặc định
        }
    }
}
