using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Service : BaseEntity
{
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public ServiceStatus Status { get; set; }
    public int? DepartmentId { get; set; }
    
    public virtual Department? Department { get; set; }
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
} 