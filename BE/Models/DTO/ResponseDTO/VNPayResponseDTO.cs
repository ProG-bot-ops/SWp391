namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class VNPayResponseDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string ResponseMessage { get; set; } = string.Empty;
} 