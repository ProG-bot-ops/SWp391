using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Data
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(Payment payment);
        Task SaveChangesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task UpdateInvoiceAsync(Invoice invoice);
    }
}
