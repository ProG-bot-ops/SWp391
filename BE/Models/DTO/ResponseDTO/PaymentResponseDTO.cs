namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class PaymentResponseDTO
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
    public int TotalInvoices { get; set; }
    public int Status { get; set; } = 1; // 1 = Completed
}

public class PaymentPagedResponseDTO
{
    public List<PaymentResponseDTO> Payments { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
} 