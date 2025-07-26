namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO
{
    public class MedicineImportDetailDTO
    {
        public int Id { get; set; }
        public int ImportId { get; set; }              
        public int MedicineId { get; set; }           
        public string BatchNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UnitId { get; set; }                
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string CreateBy { get; set; } = string.Empty;
        public string? UpdateBy { get; set; }
    }
}
