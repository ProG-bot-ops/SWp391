using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Payment;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IVNPayService
{
    Task<VNPayResponseDTO> CreatePaymentUrlAsync(VNPayRequestDTO request);
    Task<VNPayResponseDTO> ProcessPaymentReturnAsync(IQueryCollection query);
    string CreatePaymentUrl(VNPayRequestDTO request);
    bool ValidatePaymentResponse(IQueryCollection query);
    VNPayConfig GetConfig();
    Task<VNPayQRResponseDTO> CreateQRCodeAsync(VNPayQRRequestDTO request);
} 