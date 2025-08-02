using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

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

    public async Task UpdatePaymentAsync(Payment payment, bool isTestMode = false)
    {
        var existing = await _paymentRepository.GetByIdAsync(payment.Id);
        if (existing == null)
            throw new Exception("Không tìm thấy payment để cập nhật.");

        // Check if payment is completed (status = 1) - không cho phép edit payment đã hoàn thành (trừ test mode và VNPay)
        if (!isTestMode && existing.Status == PaymentStatus.Completed && existing.UpdateBy != "VNPay System")
            throw new Exception("Không thể chỉnh sửa payment đã hoàn thành.");

        // Update all fields
        Console.WriteLine($"Before update - Payment ID {existing.Id}: Status={existing.Status} ({(int)existing.Status})");
        
        existing.Payer = payment.Payer;
        existing.PaymentDate = DateTime.Now; // PaymentDate = UpdateDate (thời gian hiện tại)
        existing.PaymentMethod = payment.PaymentMethod;
        existing.Amount = payment.Amount;
        existing.Notes = payment.Notes;
        existing.Status = payment.Status;
        existing.UpdateDate = DateTime.Now;
        existing.UpdateBy = isTestMode ? "Test System" : "System";
        
        Console.WriteLine($"After update - Payment ID {existing.Id}: Status={existing.Status} ({(int)existing.Status})");

        await _paymentRepository.UpdateAsync(existing);
        Console.WriteLine($"Payment updated in memory: ID={existing.Id}, Status={existing.Status}");
        
        // Sử dụng transaction để đảm bảo commit
        using var transaction = await _paymentRepository.BeginTransactionAsync();
        try
        {
            await _paymentRepository.SaveChangesAsync();
            await transaction.CommitAsync();
            Console.WriteLine($"Payment saved to database successfully: ID={existing.Id}, Status={existing.Status}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error saving payment: {ex.Message}");
            throw;
        }
    }

    public async Task DeletePaymentAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null) return;

        // Check if payment is completed (status = 1) - không cho phép xóa payment đã hoàn thành
        if (payment.Status == PaymentStatus.Completed)
            throw new Exception("Không thể xóa payment đã hoàn thành.");

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

    public async Task<PaymentPagedResponseDTO> GetPaymentsWithFilterAsync(PaymentFilterRequest request)
    {
        return await _paymentRepository.GetPaymentsWithFilterAsync(request);
    }

    public async Task<Payment> CreatePaymentFromAppointmentAsync(AddPaymentFromAppointmentRequestDTO request)
    {
        return await _paymentRepository.CreatePaymentFromAppointmentAsync(request);
    }
} 