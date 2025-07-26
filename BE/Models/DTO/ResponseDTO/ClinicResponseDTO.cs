using static SWP391_SE1914_ManageHospital.Ultility.Status;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class ClinicResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    [EnumDataType(typeof(ClinicStatus))]
    public ClinicStatus Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }

    // Thêm trường để biết phòng khám đã kín lịch chưa
    public bool isFull { get; set; } = false;

    public ClinicResponseDTO()
    {
    }

    public ClinicResponseDTO(int id, string name, string code, ClinicStatus status)
    {
        Id = id;
        Name = name;
        Code = code;
        Status = status;
    }
}
