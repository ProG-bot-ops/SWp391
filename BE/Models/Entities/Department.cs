using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Department : BaseEntity
{
    // vi du cho truong Code: ICD-10
    public string Description { get; set; } 
    public int TotalAmountOfPeople { get; set; }
    public DepartmentStatus Status { get; set; }
    public int ClinicId { get; set; }
    public virtual Clinic Clinic { get; set; }
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    public virtual ICollection<Nurse> Nurses { get; set; } = new List<Nurse>();
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
