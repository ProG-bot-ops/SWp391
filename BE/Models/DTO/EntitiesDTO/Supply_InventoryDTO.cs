namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class Supply_InventoryDTO
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public DateTime ImportDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string SupplierName { get; set; }
    public int SupplyId { get; set; }
}
