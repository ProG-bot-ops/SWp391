using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment;

public class AppointmentCreateRequest
{
    public int ClinicId { get; set; }
    public int DoctorId { get; set; }
    public int ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Note { get; set; }
    public string Shift { get; set; } // "morning" hoặc "afternoon"
    // Thông tin bệnh nhân có thể chỉnh sửa
    public PatientInfoDto PatientInfo { get; set; }
}

public class PatientInfoDto
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Gender { get; set; }
    public DateTime Dob { get; set; }
    public string CCCD { get; set; }
    public string Address { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? Allergies { get; set; }
    public string? BloodType { get; set; }
    public string? ImageURL { get; set; }
} 