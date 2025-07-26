using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Service;

public class ServiceRequestDTO
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public ServiceStatus Status { get; set; }
    public int? DepartmentId { get; set; }
} 