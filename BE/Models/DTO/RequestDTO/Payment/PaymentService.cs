using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _paymentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _paymentRepository.GetAllAsync();
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        decimal totalAmount = 0;

        foreach (var pi in payment.Payment_Invoices)
        {
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(pi.InvoiceId);
            if (invoice == null)
                throw new Exception($"Hóa đơn có ID {pi.InvoiceId} không tồn tại.");

            totalAmount += invoice.TotalAmount;

            invoice.Status = InvoiceStatus.Paid;
            await _paymentRepository.UpdateInvoiceAsync(invoice);
        }

        payment.Amount = totalAmount;
        payment.PaymentDate = DateTime.Now;

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return payment;
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        var existing = await _paymentRepository.GetByIdAsync(payment.Id);
        if (existing == null)
            throw new Exception("Không tìm thấy payment để cập nhật.");

        existing.PaymentMethod = payment.PaymentMethod;
        existing.Payer = payment.Payer;

        await _paymentRepository.UpdateAsync(existing);
        await _paymentRepository.SaveChangesAsync();
    }

    public async Task DeletePaymentAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null) return;

        foreach (var pi in payment.Payment_Invoices)
        {
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(pi.InvoiceId);
            if (invoice != null)
            {
                invoice.Status = InvoiceStatus.Unpaid;
                await _paymentRepository.UpdateInvoiceAsync(invoice);
            }
        }

        await _paymentRepository.DeleteAsync(payment);
        await _paymentRepository.SaveChangesAsync();
    }
}
