using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class Medicine_InventoryDTO
{
    public int Id;
    public int Quantity { get; set; }
    public int BatchNumber { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime ImportDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string SupplierName { get; set; }
    public MedicineInventoryStatus Status { get; set; }
    public int MedicineId { get; set; }
}
