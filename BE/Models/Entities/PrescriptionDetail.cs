using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class PrescriptionDetail : BaseEntity
{
    public int Quantity { get; set; }
    public string Usage { get; set; }
    public PrescriptionDetailStatus Status { get; set; }
    public int PrescriptionId { get; set; }
    public int MedicineId { get; set; }
    public virtual Medicine Medicine { get; set; }
    public virtual Prescription Prescription { get; set; }
}
