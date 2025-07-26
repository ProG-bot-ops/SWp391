namespace SWP391_SE1914_ManageHospital.Service;

public interface IEmailService
{
            Task SendResetPasswordEmailAsync(string toEmail, string resetToken, string userType);
        Task SendAppointmentConfirmationEmailAsync(string toEmail, string patientName, string appointmentCode, DateTime appointmentDate, string shift, string doctorName, string clinicName);
        Task SendAppointmentReminderEmailAsync(string toEmail, string patientName, string appointmentCode, DateTime appointmentDate, string shift, string doctorName, string clinicName);
}
