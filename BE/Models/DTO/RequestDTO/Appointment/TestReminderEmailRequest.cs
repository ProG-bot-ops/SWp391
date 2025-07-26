namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Appointment
{
    public class TestReminderEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string AppointmentCode { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Shift { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
    }
} 