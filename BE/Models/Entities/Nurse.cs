using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Nurse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime Dob { get; set; }
    public string CCCD { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? ImageURL { get; set; }
    public NurseStatus Status { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string CreateBy { get; set; } = string.Empty;
    public string? UpdateBy { get; set; }

    public virtual User User { get; set; }
    public virtual Department Department { get; set; }
    public virtual ICollection<Nurse_Appointment> Nurse_Appointments { get; set; } = new List<Nurse_Appointment>();

}