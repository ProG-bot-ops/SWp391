namespace SWP391_SE1914_ManageHospital.Service;

public interface IEmailService
{
    Task SendResetPasswordEmailAsync(string toEmail, string resetToken, string userType);
    Task SendAppointmentReminderEmailAsync(string toEmail, string Name, DateTime startTime);
}
