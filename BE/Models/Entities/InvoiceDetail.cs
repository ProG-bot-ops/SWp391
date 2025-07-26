using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class InvoiceDetail : BaseEntity
{
    public InvoiceDetailStatus Status { get; set; }
    public decimal? Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public int InvoiceId { get; set; }
    public int? MedicineId { get; set; }
    public int? ServiceId { get; set; }
    public virtual Invoice Invoice { get; set; }
    public virtual Medicine? Medicine { get; set; }
    public virtual Service? Service { get; set; }
}
