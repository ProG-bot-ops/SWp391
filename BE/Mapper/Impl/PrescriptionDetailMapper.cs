using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PrescriptionDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

public class PrescriptionDetailMapper : IPrescriptionDetailMapper
{
    // Ánh xạ từ entity PrescriptionDetail sang DTO PrescriptionDetailResponseDTO
    public PrescriptionDetailResponseDTO MapToResponse(PrescriptionDetail e) => new()
    {
        Id = e.Id,
        PrescriptionId = e.PrescriptionId,
        MedicineId = e.MedicineId,
        MedicineName = e.Name,  // Lấy tên thuốc từ đối tượng Medicine
        Quantity = e.Quantity,
        Usage = e.Usage,
        Status = e.Status.ToString(),
        CreateDate = e.CreateDate,
        UpdateDate = (DateTime)e.UpdateDate,
        CreateBy = e.CreateBy!,
        UpdateBy = e.UpdateBy
    };

    // Ánh xạ từ DTO PrescriptionDetailRequest sang entity PrescriptionDetail (dùng khi thêm hoặc cập nhật chi tiết thuốc)
    public PrescriptionDetail MapToEntity(PrescriptionDetailRequest req, string userName) => new()
    {
        PrescriptionId = req.PrescriptionId,
       
        Quantity = req.Quantity,
        Usage = req.Usage,
        Status = (SWP391_SE1914_ManageHospital.Ultility.Status.PrescriptionDetailStatus)req.Status,
        CreateDate = DateTime.UtcNow,
        UpdateDate = DateTime.UtcNow,
        CreateBy = userName,
        UpdateBy = userName
    };

    // Cập nhật entity PrescriptionDetail với DTO PrescriptionDetailRequest (dùng khi cập nhật chi tiết thuốc)
    public void MapToExistingEntity(PrescriptionDetailRequest req, PrescriptionDetail e, string userName)
    {
        
        e.Quantity = req.Quantity;
        e.Usage = req.Usage;
        e.Status = (SWP391_SE1914_ManageHospital.Ultility.Status.PrescriptionDetailStatus)req.Status;
        e.UpdateDate = DateTime.UtcNow;
        e.UpdateBy = userName;
    }
}
