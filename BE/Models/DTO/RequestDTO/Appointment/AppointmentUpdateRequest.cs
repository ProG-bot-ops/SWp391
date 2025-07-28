using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment;

public class AppointmentUpdateRequest
{
    public int Id { get; set; }
    public string? PatientName { get; set; }
    public string? PatientPhone { get; set; }
    public string? PatientEmail { get; set; }
    public string? AppointmentDate { get; set; }
    public string? StartTime { get; set; }
    public string? Shift { get; set; }
    public string? Status { get; set; }
    public int? ClinicId { get; set; }
    public int? DoctorId { get; set; }
    public int? ServiceId { get; set; }
    public string? Note { get; set; }
} 