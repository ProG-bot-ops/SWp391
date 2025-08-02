using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IPaymentService
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment, bool isTestMode = false);
    Task DeletePaymentAsync(int id);
    Task<PaymentPagedResponseDTO> GetPaymentsWithFilterAsync(PaymentFilterRequest request);
    Task<Payment> CreatePaymentFromAppointmentAsync(AddPaymentFromAppointmentRequestDTO request);
}
