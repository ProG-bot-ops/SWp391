namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Supply_Inventory
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public DateTime ImportDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string SupplierName { get; set; }
    public int SupplyId { get; set; }
    public virtual Supply Supply { get; set; } = null!;
}
