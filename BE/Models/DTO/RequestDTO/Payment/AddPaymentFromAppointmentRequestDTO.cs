namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;

public class AddPaymentFromAppointmentRequestDTO
{
    public int AppointmentId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
} 