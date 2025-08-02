using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Data;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(Payment payment);
    Task SaveChangesAsync();
    Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync();
    Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
    Task UpdateInvoiceAsync(Invoice invoice);
    Task<PaymentPagedResponseDTO> GetPaymentsWithFilterAsync(PaymentFilterRequest request);
    Task<Payment> CreatePaymentFromAppointmentAsync(AddPaymentFromAppointmentRequestDTO request);
}
