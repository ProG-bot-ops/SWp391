namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineImportDetailResponseDTO
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
        public string MedicineCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CreateBy { get; set; } = string.Empty;
        public string? UpdateBy { get; set; }

        public string MedicineName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;

        public MedicineImportDetailResponseDTO() { }

        public MedicineImportDetailResponseDTO(int id, int importId, int medicineId, string batchNumber, int quantity, decimal unitPrice, DateTime manufactureDate, DateTime expiryDate, int unitId)
        {
            Id = id;
            ImportId = importId;
            MedicineId = medicineId;
            BatchNumber = batchNumber;
            Quantity = quantity;
            UnitPrice = unitPrice;
            ManufactureDate = manufactureDate;
            ExpiryDate = expiryDate;
            UnitId = unitId;
        }
    }
}
