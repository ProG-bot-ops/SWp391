using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Clinic : BaseEntity
{
    public ClinicStatus Status { get; set; }
    public ClinicType Type { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
