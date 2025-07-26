using System.ComponentModel;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.Entities;
namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Medicine : BaseEntity
{
    public string? ImageUrl { get; set; }
    public MedicineStatus Status { get; set; }
    public string Description { get; set; }
    public int UnitId { get; set; }
    public virtual Unit Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public PrescribedMedication Prescribed { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public int MedicineCategoryId { get; set; }
    public virtual MedicineCategory MedicineCategory { get; set; }
    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
    public virtual ICollection<Medicine_Inventory> Medicine_Inventories { get; set; } = new List<Medicine_Inventory>();
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    public virtual MedicineDetail MedicineDetail { get; set; }
    public virtual ICollection<MedicineImportDetail> MedicineImportDetails { get; set; } = new List<MedicineImportDetail>();


}
