using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment;

public class AppointmentUpdateRequest
{
    public int Id { get; set; }
    public string? PatientName { get; set; }
    public string? PatientEmail { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Reason { get; set; }
    public string? PatientType { get; set; }
    public string? Note { get; set; }
} 