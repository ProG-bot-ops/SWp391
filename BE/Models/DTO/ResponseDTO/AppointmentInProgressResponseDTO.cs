namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class AppointmentInProgressResponseDTO
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
} 