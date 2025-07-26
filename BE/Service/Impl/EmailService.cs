
using System.Net.Mail;
using System.Net;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendResetPasswordEmailAsync(string toEmail, string resetToken, string userType)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var fromEmail = smtpSettings["FromEmail"];
        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"]);
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];
        var websiteUrl = smtpSettings["WebsiteUrl"]?.TrimEnd('/');

        var fullUrl = $"{websiteUrl}/frontend/reset-password-form.html?token={resetToken}&userType={userType}";

        using var client = new SmtpClient
        {
            Host = host,
            Port = port,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(username, password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, "Bệnh viện Kivicare"),
            Subject = "Đặt lại mật khẩu",
            IsBodyHtml = true,
            Body = $@"
                    <h2>Đặt lại mật khẩu</h2>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản tại Colo Shop.</p>
                    <p>Click vào link bên dưới để đặt lại mật khẩu của bạn:</p>
                    <a href=""{fullUrl}"">
                        Đặt lại mật khẩu
                    </a>
                    <p>Link này sẽ hết hạn sau 1 giờ.</p>
                    <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>"
        };
        mailMessage.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi gửi email: {ex.Message}");
        }
    }

    public async Task SendAppointmentReminderEmailAsync(string toEmail, string Name, DateTime startTime)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var fromEmail = smtpSettings["FromEmail"];
        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"]);
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];

        using var client = new SmtpClient
        {
            Host = host,
            Port = port,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(username, password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, "Bệnh viện Kivicare"),
            Subject = "Nhắc lịch khám bệnh",
            IsBodyHtml = true,
            Body = $@"
            <h2>Xin chào {Name},</h2>
            <p>Đây là email nhắc nhở bạn có lịch hẹn khám lúc <b>{startTime:HH:mm dd/MM/yyyy}</b>.</p>
            <p>Vui lòng đến đúng giờ. Hãy xem lại lịch của bạn trong trang</p>
            <p>Trân trọng,</p><p>Bệnh viện Kivicare</p>"
        };

        mailMessage.To.Add(toEmail);
        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi gửi email: {ex.Message}");
        }
        
    }
}
