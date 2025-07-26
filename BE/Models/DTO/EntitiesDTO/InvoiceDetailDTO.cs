using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class InvoiceDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public InvoiceDetailStatus Status { get; set; }
    public decimal? Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public int InvoiceId { get; set; }
    public int MedicineId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
