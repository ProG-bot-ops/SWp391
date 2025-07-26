namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class Payment_InvoiceDTO
{
    public int Id { get; set; }
    public decimal AmountPaid { get; set; }
    public int PaymentId { get; set; }
    public int InvoiceId { get; set; }
}
