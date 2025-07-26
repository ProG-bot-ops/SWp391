using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IPaymentService
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
    Task DeletePaymentAsync(int id);
}
