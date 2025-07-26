namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineImportResponseDTO
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

        public MedicineImportResponseDTO() { }

        public MedicineImportResponseDTO(int id, string code, string name, string notes, int supplierId)
        {
            Id = id;
            Code = code;
            Name = name;
            Notes = notes;
            SupplierId = supplierId;
        }
    }
}
