using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class InvoiceDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public decimal InitialAmount { get; set; } //Price ban dau
    public decimal DiscountAmount { get; set; } //Price duoc giam
    public decimal TotalAmount { get; set; } //Price cuoi cung
    public string? Notes { get; set; }
    public InvoiceStatus Status { get; set; }
    public int AppointmentId { get; set; }
    public int? InsuranceId { get; set; }
    public int PatientId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
