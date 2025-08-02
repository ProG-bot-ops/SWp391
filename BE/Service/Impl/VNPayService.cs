using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

public class VNPayService : IVNPayService
{
    private readonly VNPayConfig _vnPayConfig;

    public VNPayService(IOptions<VNPayConfig> vnPayConfig)
    {
        _vnPayConfig = vnPayConfig.Value;
    }

    public VNPayConfig GetConfig()
    {
        return _vnPayConfig;
    }

    public async Task<VNPayQRResponseDTO> CreateQRCodeAsync(VNPayQRRequestDTO request)
    {
        try
        {
            // Lấy 14 số sau của Code thay vì dùng PaymentId
            var orderId = ExtractOrderIdFromCode(request.PaymentCode);
            
            // Tạo payment URL
            var paymentRequest = new VNPayRequestDTO
            {
                InvoiceId = long.Parse(orderId), // Dùng long để chứa 14 số
                Amount = request.Amount,
                OrderDescription = request.OrderInfo,
                CustomerEmail = "customer@example.com",
                CustomerPhone = "0123456789",
                CustomerName = request.Payer
            };

            var paymentUrl = CreatePaymentUrl(paymentRequest);

            return new VNPayQRResponseDTO
            {
                Success = true,
                Message = "Tạo thanh toán VNPay thành công",
                PaymentUrl = paymentUrl,
                OrderId = orderId,
                Amount = request.Amount,
                TransactionId = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VNPay Error: {ex.Message}");
            return new VNPayQRResponseDTO
            {
                Success = false,
                Message = $"Lỗi tạo thanh toán VNPay: {ex.Message}"
            };
        }
    }

    private string ExtractOrderIdFromCode(string paymentCode)
    {
        Console.WriteLine($"ExtractOrderIdFromCode called with: '{paymentCode}' (Length: {paymentCode?.Length ?? 0})");
        
        if (string.IsNullOrEmpty(paymentCode))
        {
            throw new ArgumentException("Payment code không được để trống");
        }
        
        if (paymentCode.Length < 14)
        {
            throw new ArgumentException($"Payment code quá ngắn: {paymentCode.Length} ký tự (cần ít nhất 14 ký tự)");
        }

        // Lấy 14 số cuối của Code
        var orderId = paymentCode.Substring(paymentCode.Length - 14);
        
        // Kiểm tra xem có phải toàn số không
        if (!orderId.All(char.IsDigit))
        {
            throw new ArgumentException($"14 số cuối của payment code không hợp lệ: '{orderId}'");
        }

        Console.WriteLine($"Extracted OrderId from Code '{paymentCode}': {orderId}");
        return orderId;
    }

    public async Task<VNPayResponseDTO> CreatePaymentUrlAsync(VNPayRequestDTO request)
    {
        try
        {
            var paymentUrl = CreatePaymentUrl(request);
            
            // Debug log
            Console.WriteLine($"VNPay URL created: {paymentUrl}");
            
            return new VNPayResponseDTO
            {
                Success = true,
                Message = "Tạo URL thanh toán thành công",
                PaymentUrl = paymentUrl,
                OrderId = request.InvoiceId.ToString(),
                Amount = request.Amount
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VNPay Error: {ex.Message}");
            return new VNPayResponseDTO
            {
                Success = false,
                Message = $"Lỗi tạo URL thanh toán: {ex.Message}"
            };
        }
    }

    public string CreatePaymentUrl(VNPayRequestDTO request)
    {
        var vnpay = new SortedList<string, string>(new VNPayCompare());
        
        vnpay.Add("vnp_Version", _vnPayConfig.Version);
        vnpay.Add("vnp_Command", _vnPayConfig.Command);
        vnpay.Add("vnp_TmnCode", _vnPayConfig.TmnCode);
        vnpay.Add("vnp_Amount", (request.Amount * 100).ToString()); // VNPay expects amount in VND cents
        vnpay.Add("vnp_CurrCode", _vnPayConfig.CurrCode);
        vnpay.Add("vnp_BankCode", "");
        vnpay.Add("vnp_TxnRef", request.InvoiceId.ToString()); // InvoiceId = PaymentId
        Console.WriteLine($"VNPay vnp_TxnRef (OrderId): {request.InvoiceId} (Type: {request.InvoiceId.GetType()})");
        vnpay.Add("vnp_OrderInfo", request.OrderDescription);
        vnpay.Add("vnp_OrderType", "other");
        vnpay.Add("vnp_Locale", _vnPayConfig.Locale);
        vnpay.Add("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
        vnpay.Add("vnp_IpAddr", "127.0.0.1");
        // Sử dụng timestamp cố định để tránh lỗi chữ ký
        var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        vnpay.Add("vnp_CreateDate", createDate);

        var queryString = "";
        foreach (var kv in vnpay)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                queryString += $"{kv.Key}={Uri.EscapeDataString(kv.Value)}&";
            }
        }

        queryString = queryString.TrimEnd('&');
        var signData = queryString;
        var vnp_SecureHash = HmacSHA512(_vnPayConfig.HashSecret, signData);
        queryString += $"&vnp_SecureHash={vnp_SecureHash}";

        // Debug logs
        Console.WriteLine($"=== VNPAY DEBUG INFO ===");
        Console.WriteLine($"VNPay Config - TmnCode: {_vnPayConfig.TmnCode}");
        Console.WriteLine($"VNPay Config - HashSecret: {_vnPayConfig.HashSecret}");
        Console.WriteLine($"VNPay Request - InvoiceId: {request.InvoiceId}");
        Console.WriteLine($"VNPay Request - Amount: {request.Amount}");
        Console.WriteLine($"VNPay Request - OrderDescription: {request.OrderDescription}");
        Console.WriteLine($"VNPay vnp_Amount: {(request.Amount * 100).ToString()}");
        Console.WriteLine($"VNPay vnp_CreateDate: {createDate}");
        Console.WriteLine($"VNPay SignData: {signData}");
        Console.WriteLine($"VNPay SecureHash: {vnp_SecureHash}");
        Console.WriteLine($"VNPay Final URL: {_vnPayConfig.BaseUrl}?{queryString}");
        Console.WriteLine($"=== END VNPAY DEBUG ===");

        return $"{_vnPayConfig.BaseUrl}?{queryString}";
    }

    public async Task<VNPayResponseDTO> ProcessPaymentReturnAsync(IQueryCollection query)
    {
        try
        {
            // Bỏ qua validation cho test (khi có test_hash)
            var isTestMode = query.ContainsKey("vnp_SecureHash") && query["vnp_SecureHash"].ToString() == "test_hash";
            
            if (!isTestMode && !ValidatePaymentResponse(query))
            {
                return new VNPayResponseDTO
                {
                    Success = false,
                    Message = "Chữ ký không hợp lệ"
                };
            }

            var responseCode = query["vnp_ResponseCode"].ToString();
            var orderId = query["vnp_TxnRef"].ToString();
            var transactionId = query["vnp_TransactionNo"].ToString();
            var amount = query["vnp_Amount"].ToString();
            var responseMessage = query["vnp_ResponseMessage"].ToString();
            
            Console.WriteLine($"=== VNPAY RETURN PROCESSING ===");
            Console.WriteLine($"ResponseCode: {responseCode}");
            Console.WriteLine($"OrderId (vnp_TxnRef): {orderId}");
            Console.WriteLine($"TransactionId: {transactionId}");
            Console.WriteLine($"Amount: {amount}");
            Console.WriteLine($"ResponseMessage: {responseMessage}");
            Console.WriteLine($"=== END VNPAY RETURN PROCESSING ===");

            var success = responseCode == "00";
            var amountInVND = decimal.Parse(amount) / 100; // Convert from cents to VND

            return new VNPayResponseDTO
            {
                Success = success,
                Message = success ? "Thanh toán thành công" : $"Thanh toán thất bại: {responseMessage}",
                OrderId = orderId ?? "",
                TransactionId = transactionId ?? "",
                Amount = amountInVND,
                ResponseCode = responseCode ?? "",
                ResponseMessage = responseMessage ?? ""
            };
        }
        catch (Exception ex)
        {
            return new VNPayResponseDTO
            {
                Success = false,
                Message = $"Lỗi xử lý phản hồi thanh toán: {ex.Message}"
            };
        }
    }

    public bool ValidatePaymentResponse(IQueryCollection query)
    {
        var vnp_SecureHash = query["vnp_SecureHash"].ToString();
        var vnp_SecureHashType = query["vnp_SecureHashType"].ToString();

        var signData = "";
        var sortedParams = new SortedList<string, string>(new VNPayCompare());

        foreach (var kv in query)
        {
            if (kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType" && !string.IsNullOrEmpty(kv.Value))
            {
                sortedParams.Add(kv.Key, kv.Value);
            }
        }

        foreach (var kv in sortedParams)
        {
            signData += $"{kv.Key}={Uri.EscapeDataString(kv.Value)}&";
        }

        signData = signData.TrimEnd('&');
        var checkSum = HmacSHA512(_vnPayConfig.HashSecret, signData);

        return checkSum.Equals(vnp_SecureHash, StringComparison.OrdinalIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }
        
        return hash.ToString();
    }
}

public class VNPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var vnpCompare = CompareInfo.GetCompareInfo("en-US");
        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
} 