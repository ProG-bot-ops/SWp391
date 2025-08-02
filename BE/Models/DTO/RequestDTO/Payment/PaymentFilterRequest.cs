namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;

public class PaymentFilterRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SortBy { get; set; } = "PaymentDate";
    public string? SortOrder { get; set; } = "desc";
    public int? Status { get; set; }
} 