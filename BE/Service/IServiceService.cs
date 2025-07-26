using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Service;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IServiceService
{
    Task<List<ServiceResponseDTO>> GetAllServicesAsync();
    Task<ServiceResponseDTO?> GetServiceByIdAsync(int id);
    Task<ServiceResponseDTO> CreateServiceAsync(ServiceRequestDTO request);
    Task<ServiceResponseDTO?> UpdateServiceAsync(int id, ServiceRequestDTO request);
    Task<bool> DeleteServiceAsync(int id);
    Task<List<ServiceResponseDTO>> GetServicesByDepartmentAsync(int departmentId);
} 