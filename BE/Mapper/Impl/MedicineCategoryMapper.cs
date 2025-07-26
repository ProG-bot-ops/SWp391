using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public class MedicineCategoryMapper : IMedicineCategoryMapper
    {
        public MedicineCategoryResponseDTO MapToDTO(MedicineCategory entity)
        {
            return new MedicineCategoryResponseDTO
            {
                Id = entity.Id,
                ImageUrl = entity.ImageUrl,
                Description = entity.Description,
                Status = entity.Status,    // nếu bạn muốn hiện "Active", "Inactive",...
                Name = entity.Name,
                Code = entity.Code,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CreateBy = entity.CreateBy ?? "system",
                UpdateBy = entity.UpdateBy ?? "system"
            };
        }


        public MedicineCategory MapToEntity(MedicineCategoryRequest request)
        {
            return new MedicineCategory
            {
                ImageUrl = request.ImageUrl,
                Description = request.Description,
                Status = (MedicineCategoryStatus)request.Status,
                Name = request.Name,
                Code = request.Code,
                CreateDate = DateTime.UtcNow,               // Thời gian thực
                CreateBy = "admin",                         // Hoặc user đang login
                UpdateDate = DateTime.UtcNow,
                UpdateBy = "admin"
            };
        }


        public void MapToExistingEntity(MedicineCategoryRequest request, MedicineCategory entity)
        {
            entity.ImageUrl = request.ImageUrl;
            entity.Description = request.Description;
            entity.Status = (MedicineCategoryStatus)request.Status;
            entity.Name = request.Name;
            entity.Code = request.Code;
            entity.UpdateDate = DateTime.UtcNow;
            entity.UpdateBy = "admin"; // hoặc lấy từ token người dùng
        }

    }
}
