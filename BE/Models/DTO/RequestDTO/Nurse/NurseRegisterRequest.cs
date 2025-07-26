using static SWP391_SE1914_ManageHospital.Ultility.Status;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;

public class NurseRegisterRequest
{
    // USER
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    // PATIENT
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Code { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public DateTime Dob { get; set; }

    [Required]
    [RegularExpression(@"\d{12}")]
    public string CCCD { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    public int DepartmentId { get; set; } 

}
