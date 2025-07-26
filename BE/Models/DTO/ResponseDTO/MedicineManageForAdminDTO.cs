namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineManageForAdminDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string Ingredients { get; set; }
        public decimal LatestUnitPrice { get; set; }     
        public int TotalQuantityInStock { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string SupplierName { get; set; } = string.Empty;

    }
}
