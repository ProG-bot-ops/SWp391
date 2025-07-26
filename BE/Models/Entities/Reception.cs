using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Reception : BaseEntity
{
    public Gender Gender { get; set; }
    public DateTime Dob { get; set; }
    public string CCCD { get; set; }
    public string Phone { get; set; }
    public string? ImageURL { get; set; }
    public ReceptionStatus Status { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
