using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment;

public class AppointmentUpdateRequest
{
    public DateTime? AppointmentDate { get; set; }
    public string? Shift { get; set; } // "morning" hoặc "afternoon"
    public string? Note { get; set; }
    public string? Status { get; set; }
} 