using static SWP391_SE1914_ManageHospital.Ultility.Status;
namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Insurance : BaseEntity
{
    public string Description { get; set; }
    public InsuranceStatus Status { get; set; }
    public int CoveragePercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PatientId { get; set; }
    public virtual Patient Patient { get; set; } = null!;
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
