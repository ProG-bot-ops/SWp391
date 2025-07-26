namespace SWP391_SE1914_ManageHospital.Models.Entities
{
    public class MedicineImport : BaseEntity
    {
        public string Notes { get; set; }

        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<MedicineImportDetail> MedicineImportDetails { get; set; } = new List<MedicineImportDetail>();
    }
}
