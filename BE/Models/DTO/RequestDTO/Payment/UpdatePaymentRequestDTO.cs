using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;

public class UpdatePaymentRequestDTO
{
    [Required(ErrorMessage = "ID thanh toán là bắt buộc")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
    public string Payer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày thanh toán là bắt buộc")]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
    [StringLength(50, ErrorMessage = "Phương thức thanh toán không được vượt quá 50 ký tự")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số tiền là bắt buộc")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
    public decimal Amount { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Trạng thái thanh toán là bắt buộc")]
    public int Status { get; set; } = 1; // 1 = Completed
} 