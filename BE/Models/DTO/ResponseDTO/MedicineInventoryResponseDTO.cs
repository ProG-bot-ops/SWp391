using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineInventoryResponseDTO
    {
        public int Id { get; set; }       
        public string MedicineCode { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public MedicineInventoryStatus Status { get; set; }
        public int Quantity { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime? ImportDate { get; set; }
    }
}
