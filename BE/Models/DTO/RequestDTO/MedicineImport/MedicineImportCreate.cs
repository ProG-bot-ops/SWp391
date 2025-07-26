namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport
{
    public class MedicineImportCreate
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int SupplierId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string? UpdateBy { get; set; }

        public MedicineImportCreate() { }

        public MedicineImportCreate(int id, string code, string name, string notes, int supplierId, DateTime createDate, DateTime? updateDate, string createBy, string? updateBy)
        {
            Id = id;
            this.Code = code;
            Name = name;
            Notes = notes;
            SupplierId = supplierId;
            CreateDate = createDate;
            UpdateDate = updateDate;
            CreateBy = createBy;
            UpdateBy = updateBy;
        }
    }
}
