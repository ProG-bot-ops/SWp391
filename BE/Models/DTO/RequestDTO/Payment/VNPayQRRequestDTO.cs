namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;

public class VNPayQRRequestDTO
{
    public int PaymentId { get; set; }
    public string PaymentCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Payer { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public string OrderType { get; set; } = "billpayment";
} 