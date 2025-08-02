namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;

public class VNPayRequestDTO
{
    public long InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string OrderDescription { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
} 