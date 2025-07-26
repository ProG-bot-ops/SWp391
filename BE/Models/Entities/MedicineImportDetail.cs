namespace SWP391_SE1914_ManageHospital.Models.Entities
{
    public class MedicineImportDetail : BaseEntity
    {
        public int ImportId { get; set; }
        public virtual MedicineImport Import { get; set; } = null!;

        public int MedicineId { get; set; }
        public virtual Medicine Medicine { get; set; } = null!;

        public string BatchNumber { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; } = null!;

        public virtual ICollection<Medicine_Inventory> Inventories { get; set; } = new List<Medicine_Inventory>();

    }
}
