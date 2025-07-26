namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class PaymentDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Payer { get; set; }
    public string? Notes { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
}
