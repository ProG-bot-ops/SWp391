using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

public class NotificationDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime SendTime { get; set; }
    public int UserId { get; set; }
}
