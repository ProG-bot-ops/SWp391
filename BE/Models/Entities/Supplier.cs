namespace SWP391_SE1914_ManageHospital.Models.Entities
{
    public class Supplier : BaseEntity
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public virtual ICollection<MedicineImport> MedicineImports { get; set; } = new List<MedicineImport>();
    }
}
