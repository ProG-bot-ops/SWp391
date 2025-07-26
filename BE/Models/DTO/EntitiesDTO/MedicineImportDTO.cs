namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO
{
    public class MedicineImportDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int SupplierId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string? UpdateBy { get; set; }

    }
}
