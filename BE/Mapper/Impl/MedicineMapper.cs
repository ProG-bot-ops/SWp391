using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using DocumentFormat.OpenXml.Vml.Office;

public class MedicineMapper : IMedicineMapper
{
    public MedicineResponseDTO MapToDTO(Medicine entity)
    {
        return new MedicineResponseDTO
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            Description = entity.Description,
            Status = (MedicineStatus)entity.Status,
            Name = entity.Name,
            Code = entity.Code,
            Dosage = entity.Dosage,
            UnitId = entity.UnitId,
            UnitPrice = entity.UnitPrice,
            MedicineCategoryId = entity.MedicineCategoryId,
            Prescribed = entity.Prescribed.ToString(),
            CreateDate = entity.CreateDate,
            UpdateDate = entity.UpdateDate,
            CreateBy = entity.CreateBy ?? "system",
            UpdateBy = entity.UpdateBy ?? "system"
        };
    }

    public Medicine MapToEntity(MedicineRequest request)
    {
        return new Medicine
        {
            ImageUrl = request.ImageUrl,
            Description = request.Description,
            Status = (MedicineStatus)request.Status,
            Name = request.Name,
            Code = request.Code,
            Dosage = request.Dosage,
            UnitId = request.UnitId,
            UnitPrice = request.UnitPrice,
            MedicineCategoryId = request.MedicineCategoryId,
            CreateDate = DateTime.UtcNow,
            CreateBy = "system",         // ✅ bắt buộc
            UpdateDate = DateTime.UtcNow,
            UpdateBy = "system"          // ✅ thêm luôn nếu DB không cho null
        };
    }

    public void MapToExistingEntity(MedicineRequest request, Medicine entity)
    {
        entity.ImageUrl = request.ImageUrl;
        entity.Description = request.Description;
        entity.Status = (MedicineStatus)request.Status;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Dosage = request.Dosage;
        entity.UnitId = request.UnitId;
        entity.UnitPrice = request.UnitPrice;
        entity.MedicineCategoryId = request.MedicineCategoryId;
        entity.UpdateDate = DateTime.UtcNow;
        entity.UpdateBy = "system";
    }
}
