using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class PrescriptionDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int Quantity { get; set; }
    public string Usage { get; set; }
    public PrescriptionDetailStatus Status { get; set; }
    public int PrescriptionId { get; set; }
    public int MedicineId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
