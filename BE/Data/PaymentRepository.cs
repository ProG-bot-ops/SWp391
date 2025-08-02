using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Ultility;
using PaymentStatus = SWP391_SE1914_ManageHospital.Ultility.Status.PaymentStatus;

namespace SWP391_SE1914_ManageHospital.Data;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDBContext _context;

    public PaymentRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        try
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (payment != null)
            {
                // Ensure Status has a default value if it's null
                if (payment.Status == null)
                {
                    payment.Status = PaymentStatus.Completed;
                }
            }
            
            return payment;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByIdAsync for ID {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .ToListAsync();
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public async Task UpdateAsync(Payment payment)
    {
        Console.WriteLine($"PaymentRepository.UpdateAsync called: ID={payment.Id}, Status={payment.Status}");
        _context.Payments.Update(payment);
        Console.WriteLine($"Payment marked as updated in context: ID={payment.Id}");
    }

    public async Task DeleteAsync(Payment payment)
    {
        _context.Payments.Remove(payment);
    }

    public async Task SaveChangesAsync()
    {
        Console.WriteLine("PaymentRepository.SaveChangesAsync called");
        var result = await _context.SaveChangesAsync();
        Console.WriteLine($"SaveChangesAsync completed: {result} entities affected");
    }

    public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync()
    {
        Console.WriteLine("PaymentRepository.BeginTransactionAsync called");
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == invoiceId);
    }

    public async Task UpdateInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
    }

    public async Task<PaymentPagedResponseDTO> GetPaymentsWithFilterAsync(PaymentFilterRequest request)
    {
        var query = _context.Payments
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Payer.ToLower().Contains(searchTerm) ||
                p.Notes.ToLower().Contains(searchTerm) ||
                p.PaymentMethod.ToLower().Contains(searchTerm)
            );
        }

        // Apply payment method filter
        if (!string.IsNullOrEmpty(request.PaymentMethod))
        {
            query = query.Where(p => p.PaymentMethod == request.PaymentMethod);
        }

        // Apply date range filter
        if (request.FromDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate <= request.ToDate.Value);
        }

        // Apply status filter
        if (request.Status.HasValue)
        {
            query = query.Where(p => (int)p.Status == request.Status.Value);
        }

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "payer" => request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Payer)
                : query.OrderBy(p => p.Payer),
            "amount" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Amount)
                : query.OrderBy(p => p.Amount),
            "paymentmethod" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.PaymentMethod)
                : query.OrderBy(p => p.PaymentMethod),
            _ => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.PaymentDate)
                : query.OrderBy(p => p.PaymentDate)
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var payments = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // Map to DTOs
        var paymentDtos = payments.Select(p => new PaymentResponseDTO
        {
            Id = p.Id,
            Name = p.Payer,
            Code = $"PAY{p.CreateDate:yyyyMMddHHmmss}",
            PaymentDate = p.PaymentDate,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            Payer = p.Payer,
            Notes = p.Notes,
            CreateDate = p.CreateDate,
            UpdateDate = p.UpdateDate,
            CreateBy = p.CreateBy,
            UpdateBy = p.UpdateBy,
            TotalInvoices = 0, // Không sử dụng Payment_Invoices nữa
            Status = (int)p.Status
        }).ToList();

        // Calculate pagination info
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PaymentPagedResponseDTO
        {
            Payments = paymentDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasPreviousPage = request.Page > 1,
            HasNextPage = request.Page < totalPages
        };
    }

    public async Task<Payment> CreatePaymentFromAppointmentAsync(AddPaymentFromAppointmentRequestDTO request)
    {
        try
        {
            // Lấy thông tin cuộc hẹn
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

            if (appointment == null)
                throw new Exception("Không tìm thấy cuộc hẹn");

            if (appointment.Status != Ultility.Status.AppointmentStatus.InProgress)
                throw new Exception("Cuộc hẹn không ở trạng thái đang khám");

            if (!appointment.ServiceId.HasValue)
                throw new Exception("Cuộc hẹn không có dịch vụ");

            // Tạo payment mới - chỉ lưu vào bảng Payment
            var payment = new Payment
            {
                Code = $"PAY{DateTime.Now:yyyyMMddHHmmss}",
                Name = $"Thanh toán cho cuộc hẹn {appointment.Code}",
                Payer = appointment.Patient.Name,
                PaymentDate = DateTime.Now,
                PaymentMethod = request.PaymentMethod,
                Amount = appointment.Service.Price,
                Notes = request.Notes ?? $"Thanh toán cho cuộc hẹn {appointment.Code}",
                Status = Ultility.Status.PaymentStatus.Pending,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "System",
                UpdateBy = "System"
            };

            // Thêm payment vào database
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return payment;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tạo payment từ appointment: {ex.Message}. Inner Exception: {ex.InnerException?.Message}");
        }
    }
} 