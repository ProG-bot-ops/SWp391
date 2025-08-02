using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;
using SWP391_SE1914_ManageHospital.Data;
using PaymentStatus = SWP391_SE1914_ManageHospital.Ultility.Status.PaymentStatus;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IVNPayService _vnPayService;

    public PaymentController(IPaymentService paymentService, IPaymentRepository paymentRepository, IVNPayService vnPayService)
    {
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
        _vnPayService = vnPayService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetPaymentsWithFilter([FromQuery] PaymentFilterRequest request)
    {
        try
        {
            var result = await _paymentService.GetPaymentsWithFilterAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            Console.WriteLine($"Getting payment with ID: {id}");
            var payment = await _paymentService.GetByIdAsync(id);
            
            if (payment == null)
            {
                Console.WriteLine($"Payment with ID {id} not found");
                return NotFound();
            }
            
            Console.WriteLine($"Payment found: ID={payment.Id}, Payer={payment.Payer}, Amount={payment.Amount}");
            return Ok(payment);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting payment {id}: {ex.Message}");
            return StatusCode(500, new { 
                message = "Lỗi khi lấy thông tin thanh toán", 
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }

    [HttpGet("statuses")]
    public IActionResult GetPaymentStatuses()
    {
        var statuses = new[]
        {
            new { Value = 0, Text = "Đang chờ", Color = "warning" },
            new { Value = 1, Text = "Hoàn thành", Color = "success" },
            new { Value = 2, Text = "Thất bại", Color = "danger" },
            new { Value = 3, Text = "Hoàn tiền", Color = "info" }
        };
        return Ok(statuses);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddPaymentRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var payment = new Payment
            {
                Payer = request.Payer,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod,
                Amount = request.Amount,
                Notes = request.Notes,
                Status = (PaymentStatus)request.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "System",
                UpdateBy = "System"
            };

            var createdPayment = await _paymentService.CreatePaymentAsync(payment);
            return CreatedAtAction(nameof(GetById), new { id = createdPayment.Id }, createdPayment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("from-appointment")]
    public async Task<IActionResult> CreateFromAppointment([FromBody] AddPaymentFromAppointmentRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdPayment = await _paymentService.CreatePaymentFromAppointmentAsync(request);
            return Ok(new
            {
                success = true,
                message = "Tạo hóa đơn thanh toán thành công",
                data = createdPayment
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new 
            { 
                success = false,
                message = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id != request.Id)
            return BadRequest("Id không khớp");

        var existingPayment = await _paymentService.GetByIdAsync(id);
        if (existingPayment == null)
            return NotFound();

        try
        {
            existingPayment.Payer = request.Payer;
            existingPayment.PaymentDate = request.PaymentDate;
            existingPayment.PaymentMethod = request.PaymentMethod;
            existingPayment.Amount = request.Amount;
            existingPayment.Notes = request.Notes;
            existingPayment.Status = (PaymentStatus)request.Status;
            existingPayment.UpdateDate = DateTime.Now;
            existingPayment.UpdateBy = "System";

            await _paymentService.UpdatePaymentAsync(existingPayment);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound();

        await _paymentService.DeletePaymentAsync(id);
        return NoContent();
    }

    [HttpPost("vnpay-create-payment")]
    public async Task<IActionResult> CreateVNPayPayment([FromBody] VNPayRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _vnPayService.CreatePaymentUrlAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("vnpay-qr-code")]
    public async Task<IActionResult> CreateVNPayQRCode([FromBody] VNPayQRRequestDTO request)
    {
        Console.WriteLine("=== VNPAY QR CODE ENDPOINT CALLED ===");
        Console.WriteLine($"Request received: {System.Text.Json.JsonSerializer.Serialize(request)}");
        Console.WriteLine($"Request.PaymentId: {request.PaymentId} (Type: int)");
        Console.WriteLine($"Request.PaymentCode: {request.PaymentCode} (Type: string, Length: {request.PaymentCode?.Length ?? 0})");
        Console.WriteLine($"Request.Amount: {request.Amount} (Type: decimal)");
        Console.WriteLine($"Request.Payer: {request.Payer}");
        Console.WriteLine($"Request.OrderInfo: {request.OrderInfo}");
        Console.WriteLine($"Request.OrderType: {request.OrderType}");
        
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState is invalid");
            return BadRequest(ModelState);
        }

        try
        {
            Console.WriteLine("Calling VNPay service...");
            var response = await _vnPayService.CreateQRCodeAsync(request);
            Console.WriteLine($"VNPay service response: {System.Text.Json.JsonSerializer.Serialize(response)}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VNPay QR Code error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("vnpay-test")]
    public async Task<IActionResult> TestVNPay()
    {
        try
        {
            var testRequest = new VNPayRequestDTO
            {
                InvoiceId = 12345,
                Amount = 100000,
                OrderDescription = "Test payment",
                CustomerEmail = "test@example.com",
                CustomerPhone = "0123456789",
                CustomerName = "Test Customer"
            };

            var response = await _vnPayService.CreatePaymentUrlAsync(testRequest);
            return Ok(new { 
                success = true, 
                message = "VNPay test successful",
                paymentUrl = response.PaymentUrl,
                config = _vnPayService.GetConfig()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = $"VNPay test failed: {ex.Message}",
                stackTrace = ex.StackTrace
            });
        }
    }

    [HttpGet("vnpay-test-payment/{paymentId}")]
    public async Task<IActionResult> TestVNPayWithPayment(int paymentId)
    {
        try
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);
            if (payment == null)
            {
                return BadRequest(new { success = false, message = $"Payment with ID {paymentId} not found" });
            }

            var testRequest = new VNPayRequestDTO
            {
                InvoiceId = payment.Id,
                Amount = payment.Amount,
                OrderDescription = $"Payment for {payment.Payer}",
                CustomerEmail = "test@example.com",
                CustomerPhone = "0123456789",
                CustomerName = payment.Payer ?? "Unknown"
            };

            var response = await _vnPayService.CreatePaymentUrlAsync(testRequest);
            return Ok(new { 
                success = true, 
                message = "VNPay test with real payment successful",
                paymentId = payment.Id,
                paymentCode = payment.Code,
                paymentUrl = response.PaymentUrl,
                config = _vnPayService.GetConfig()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = $"VNPay test failed: {ex.Message}",
                stackTrace = ex.StackTrace
            });
        }
    }

    [HttpGet("create-test-payment")]
    public async Task<IActionResult> CreateTestPayment()
    {
        try
        {
            var payment = new Payment
            {
                Code = $"PAY{DateTime.Now:yyyyMMddHHmmss}",
                Name = "Test Payment for VNPay",
                Payer = "Test Customer",
                PaymentDate = DateTime.Now,
                PaymentMethod = "Chuyển khoản",
                Amount = 400000,
                Notes = "Test payment for VNPay integration",
                Status = PaymentStatus.Pending,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "Test System",
                UpdateBy = "Test System"
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return Ok(new { 
                success = true, 
                message = "Test payment created successfully",
                paymentId = payment.Id,
                paymentCode = payment.Code,
                status = payment.Status
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                success = false, 
                message = $"Error creating test payment: {ex.Message}"
            });
        }
    }

    [HttpGet("vnpay-test-return")]
    public async Task<IActionResult> TestVNPayReturn()
    {
        Console.WriteLine("=== TEST VNPAY RETURN ENDPOINT CALLED ===");
        try
        {
            // Simulate VNPay return with success
            var query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["vnp_ResponseCode"] = "00",
                ["vnp_TxnRef"] = "20250802204439", // OrderId từ payment thật (14 số cuối của Code)
                ["vnp_TransactionNo"] = "15113553",
                ["vnp_Amount"] = "40000000", // 400.000 VND in cents
                ["vnp_ResponseMessage"] = "Success",
                ["vnp_SecureHash"] = "test_hash",
                ["vnp_SecureHashType"] = "SHA256"
            });

            var response = await _vnPayService.ProcessPaymentReturnAsync(query);
            Console.WriteLine($"Test VNPay response: Success={response.Success}, OrderId={response.OrderId}, Message={response.Message}");
            
            if (response.Success)
            {
                // Tìm payment bằng Code (14 số cuối)
                var payments = await _paymentService.GetAllAsync();
                var payment = payments.FirstOrDefault(p => 
                    !string.IsNullOrEmpty(p.Code) && 
                    p.Code.Length >= 14 && 
                    p.Code.Substring(p.Code.Length - 14) == response.OrderId);
                
                if (payment != null)
                {
                    Console.WriteLine($"Found payment by Code: ID={payment.Id}, Code={payment.Code}, Current Status={payment.Status}");
                    payment.Status = PaymentStatus.Completed;
                    payment.UpdateDate = DateTime.Now;
                    payment.UpdateBy = "VNPay System";
                    await _paymentService.UpdatePaymentAsync(payment, true); // isTestMode = true
                    Console.WriteLine($"Payment {payment.Id} (Code: {payment.Code}) status updated to Completed successfully");
                    
                    return Ok(new { 
                        success = true, 
                        message = "Test VNPay return successful - Payment status updated",
                        paymentId = payment.Id,
                        paymentCode = payment.Code,
                        oldStatus = "Pending",
                        newStatus = "Completed"
                    });
                }
                else
                {
                    Console.WriteLine($"Payment with Code ending '{response.OrderId}' not found");
                    return BadRequest(new { 
                        success = false, 
                        message = $"Payment with Code ending '{response.OrderId}' not found",
                        availablePayments = payments.Select(p => new { p.Id, p.Code }).ToList()
                    });
                }
            }
            else
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Test VNPay return failed",
                    response = response
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test VNPay return error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { 
                success = false, 
                message = $"Test VNPay return error: {ex.Message}",
                stackTrace = ex.StackTrace
            });
        }
    }

    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VNPayReturn()
    {
        Console.WriteLine("=== VNPAY RETURN ENDPOINT CALLED ===");
        Console.WriteLine($"Request URL: {Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
        Console.WriteLine($"Request Method: {Request.Method}");
        Console.WriteLine($"Request Headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}"))}");
        
        try
        {
            var query = Request.Query;
            Console.WriteLine($"Query parameters: {string.Join(", ", query.Select(q => $"{q.Key}={q.Value}"))}");
            
            var response = await _vnPayService.ProcessPaymentReturnAsync(query);
            Console.WriteLine($"VNPay response: Success={response.Success}, OrderId={response.OrderId}, Message={response.Message}");
            
            if (response.Success)
            {
                // Cập nhật status payment thành "Hoàn thành"
                try
                {
                    var orderId = response.OrderId;
                    Console.WriteLine($"Processing orderId: {orderId}");
                    
                    // Tìm payment bằng Code (14 số cuối)
                    var payments = await _paymentService.GetAllAsync();
                    var payment = payments.FirstOrDefault(p => 
                        !string.IsNullOrEmpty(p.Code) && 
                        p.Code.Length >= 14 && 
                        p.Code.Substring(p.Code.Length - 14) == orderId);
                    
                    if (payment != null)
                    {
                        Console.WriteLine($"Found payment by Code: ID={payment.Id}, Code={payment.Code}, Current Status={payment.Status}");
                        payment.Status = PaymentStatus.Completed;
                        payment.UpdateDate = DateTime.Now;
                        payment.UpdateBy = "VNPay System";
                        await _paymentService.UpdatePaymentAsync(payment, false); // isTestMode = false cho VNPay thật
                        Console.WriteLine($"Payment {payment.Id} status updated to Completed in VNPayReturn endpoint");
                        Console.WriteLine($"Payment {payment.Id} (Code: {payment.Code}) status updated to Completed successfully");
                    }
                    else
                    {
                        Console.WriteLine($"Payment with Code ending '{orderId}' not found");
                    }
                }
                catch (Exception updateEx)
                {
                    Console.WriteLine($"Error updating payment status: {updateEx.Message}");
                    Console.WriteLine($"Stack trace: {updateEx.StackTrace}");
                    // Không throw exception để không ảnh hưởng đến redirect
                }
                
                // Redirect to payment page with success message
                var redirectUrl = $"http://127.0.0.1:5500/FE/dashboard/payment.html?status=success&orderId={response.OrderId}&amount={response.Amount}&transactionId={response.TransactionId}";
                Console.WriteLine($"Redirecting to: {redirectUrl}");
                return Redirect(redirectUrl);
            }
            else
            {
                // Redirect to payment page with error message
                var redirectUrl = $"http://127.0.0.1:5500/FE/dashboard/payment.html?status=error&message={Uri.EscapeDataString(response.Message)}";
                Console.WriteLine($"Redirecting to: {redirectUrl}");
                return Redirect(redirectUrl);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VNPay return error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            var redirectUrl = $"http://127.0.0.1:5500/FE/dashboard/payment.html?status=error&message={Uri.EscapeDataString(ex.Message)}";
            return Redirect(redirectUrl);
        }
    }

    [HttpPost("make-payment")]
    public async Task<IActionResult> MakePayment([FromBody] AddPaymentRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var payment = new Payment
            {
                Payer = request.Payer,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod,
                Amount = request.Amount,
                Notes = request.Notes,
                Status = (PaymentStatus)request.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = "System",
                UpdateBy = "System"
            };

            var createdPayment = await _paymentService.CreatePaymentAsync(payment);
            return CreatedAtAction(nameof(GetById), new { id = createdPayment.Id }, createdPayment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
