using System.ComponentModel.DataAnnotations;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

public class DoctorUpdate
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    [EnumDataType(typeof(Gender))] public Gender? Gender { get; set; }
    public DateTime? Dob { get; set; }
    public string? CCCD { get; set; }
    public string? Phone { get; set; }
    public string? ImageURL { get; set; }
    public string? LicenseNumber { get; set; }
    public float? YearOfExperience { get; set; }
    public float? WorkingHours { get; set; }
    [EnumDataType(typeof(DoctorStatus))] public DoctorStatus? Status { get; set; }
    public int? UserId { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? CreateBy { get; set; }
    public string? UpdateBy { get; set; }

    public DoctorUpdate() { }
    public DoctorUpdate(int id, string? name = null, string? code = null, Gender? gender = null, DateTime? dob = null, string? cCCD = null, string? phone = null, string? imageURL = null, string? licenseNumber = null, float? yearOfExperience = null, float? workingHours = null, DoctorStatus? status = null, int? userId = null, int? departmentId = null, DateTime? createDate = null, DateTime? updateDate = null, string? createBy = null, string? updateBy = null)
    {
        Id = id;
        Name = name;
        Code = code;
        Gender = gender;
        Dob = dob;
        CCCD = cCCD;
        Phone = phone;
        ImageURL = imageURL;
        LicenseNumber = licenseNumber;
        YearOfExperience = yearOfExperience;
        WorkingHours = workingHours;
        Status = status;
        UserId = userId;
        DepartmentId = departmentId;
        CreateDate = createDate;
        UpdateDate = updateDate;
        CreateBy = createBy;
        UpdateBy = updateBy;
    }
}