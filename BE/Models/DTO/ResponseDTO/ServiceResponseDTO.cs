using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class ServiceResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public ServiceStatus Status { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
} 