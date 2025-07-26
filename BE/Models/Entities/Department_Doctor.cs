namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Department_Doctor
{
    public int DoctorId { get; set; }
    public int DepartmentId { get; set; }
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Department Department { get; set; } = null!;
}
