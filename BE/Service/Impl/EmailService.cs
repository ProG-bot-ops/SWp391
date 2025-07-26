
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
            From = new MailAddress(fromEmail, "G-Care Clinic"),
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



    public async Task SendAppointmentConfirmationEmailAsync(string toEmail, string patientName, string appointmentCode, DateTime appointmentDate, string shift, string doctorName, string clinicName)
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

        // Chuyển đổi shift thành text tiếng Việt
        var shiftText = shift?.ToLower() switch
        {
            "morning" => "Ca sáng (07:00 - 12:00)",
            "afternoon" => "Ca chiều (13:00 - 17:00)",
            _ => shift ?? "Chưa xác định"
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, "G-Care Clinic"),
            Subject = $"Xác nhận đặt lịch hẹn - {appointmentCode}",
            IsBodyHtml = true,
            Body = $@"
            <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;"">
                <div style=""text-align: center; background-color: #4CAF50; color: white; padding: 20px; border-radius: 8px 8px 0 0;"">
                    <h1 style=""margin: 0;"">✅ Đặt lịch hẹn thành công!</h1>
                </div>
                
                <div style=""padding: 20px; background-color: #f9f9f9;"">
                    <h2 style=""color: #333;"">Xin chào {patientName},</h2>
                    <p style=""color: #666; line-height: 1.6;"">Cảm ơn bạn đã đặt lịch hẹn khám bệnh tại Phòng khám G-Care. Dưới đây là thông tin chi tiết về lịch hẹn của bạn:</p>
                    
                    <div style=""background-color: white; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #4CAF50;"">
                        <h3 style=""color: #4CAF50; margin-top: 0;"">📋 Thông tin lịch hẹn</h3>
                        <table style=""width: 100%; border-collapse: collapse;"">
                            <tr>
                                <td style=""padding: 8px 0; font-weight: bold; color: #333;"">Mã lịch hẹn:</td>
                                <td style=""padding: 8px 0; color: #666;"">{appointmentCode}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 8px 0; font-weight: bold; color: #333;"">Ngày khám:</td>
                                <td style=""padding: 8px 0; color: #666;"">{appointmentDate:dd/MM/yyyy}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 8px 0; font-weight: bold; color: #333;"">Ca khám:</td>
                                <td style=""padding: 8px 0; color: #666;"">{shiftText}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 8px 0; font-weight: bold; color: #333;"">Bác sĩ:</td>
                                <td style=""padding: 8px 0; color: #666;"">{doctorName}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 8px 0; font-weight: bold; color: #333;"">Phòng khám:</td>
                                <td style=""padding: 8px 0; color: #666;"">{clinicName}</td>
                            </tr>
                        </table>
                    </div>
                    
                    
                    
                    <p style=""color: #666; line-height: 1.6;"">Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email nguyenducgiangqp@gmail.com hoặc số điện thoại 0862502458.</p>
                    
                    <p style=""color: #666; line-height: 1.6;"">Trân trọng,<br><strong>Phòng khám G-Care</strong></p>
                </div>
                
                <div style=""text-align: center; background-color: #f8f9fa; padding: 15px; border-radius: 0 0 8px 8px; color: #666; font-size: 12px;"">
                </div>
            </div>"
        };

        mailMessage.To.Add(toEmail);
        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi gửi email xác nhận: {ex.Message}");
        }
    }

    public async Task SendAppointmentReminderEmailAsync(string toEmail, string patientName, string appointmentCode, DateTime appointmentDate, string shift, string doctorName, string clinicName)
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

        // Chuyển đổi shift thành text tiếng Việt
        var shiftText = shift?.ToLower() switch
        {
            "morning" => "Ca sáng (07:00 - 12:00)",
            "afternoon" => "Ca chiều (13:00 - 17:00)",
            _ => shift ?? "Chưa xác định"
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, "Phòng khám G-Care"),
            Subject = $"Nhắc nhở lịch hẹn - {appointmentCode}",
            IsBodyHtml = true,
            Body = $@"
            <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;"">
                <div style=""text-align: center; background-color: #ff9800; color: white; padding: 20px; border-radius: 8px 8px 0 0;"">
                    <h1 style=""margin: 0;"">⏰ Nhắc nhở lịch hẹn!</h1>
                </div>
                <div style=""padding: 20px; background-color: #f9f9f9;"">
                    <h2 style=""color: #333;"">Xin chào {patientName},</h2>
                    <p style=""color: #666; line-height: 1.6;"">Đây là email nhắc nhở về lịch hẹn khám bệnh của bạn tại Phòng khám G-Care. Vui lòng xem lại thông tin chi tiết:</p>
                    
                    <div style=""background-color: white; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #ff9800;"">
                        <h3 style=""color: #ff9800; margin-top: 0;"">📋 Thông tin lịch hẹn</h3>
                        <table style=""width: 100%; border-collapse: collapse;"">
                            <tr><td style=""padding: 8px 0; font-weight: bold; color: #333;"">Mã lịch hẹn:</td><td style=""padding: 8px 0; color: #666;"">{appointmentCode}</td></tr>
                            <tr><td style=""padding: 8px 0; font-weight: bold; color: #333;"">Ngày khám:</td><td style=""padding: 8px 0; color: #666;"">{appointmentDate:dd/MM/yyyy}</td></tr>
                            <tr><td style=""padding: 8px 0; font-weight: bold; color: #333;"">Ca khám:</td><td style=""padding: 8px 0; color: #666;"">{shiftText}</td></tr>
                            <tr><td style=""padding: 8px 0; font-weight: bold; color: #333;"">Bác sĩ:</td><td style=""padding: 8px 0; color: #666;"">{doctorName}</td></tr>
                            <tr><td style=""padding: 8px 0; font-weight: bold; color: #333;"">Phòng khám:</td><td style=""padding: 8px 0; color: #666;"">{clinicName}</td></tr>
                        </table>
                    </div>
                    
                    <div style=""background-color: #e3f2fd; border: 1px solid #2196f3; border-radius: 5px; padding: 15px; margin: 20px 0;"">
                        <h4 style=""color: #1976d2; margin-top: 0;"">💡 Lưu ý:</h4>
                        <p style=""color: #1976d2; margin: 10px 0;"">Vui lòng đến trước giờ hẹn 15 phút để làm thủ tục và chuẩn bị sẵn sàng cho buổi khám.</p>
                    </div>
                    
                    <p style=""color: #666; line-height: 1.6;"">Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email nguyenducgiangqp@gmail.com hoặc số điện thoại 0862502458.</p>
                    
                    <p style=""color: #666; line-height: 1.6;"">Trân trọng,<br><strong>Phòng khám G-Care</strong></p>
                </div>
                <div style=""text-align: center; background-color: #f8f9fa; padding: 15px; border-radius: 0 0 8px 8px; color: #666; font-size: 12px;"">
                </div>
            </div>"
        };

        mailMessage.To.Add(toEmail);
        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi gửi email nhắc nhở: {ex.Message}");
        }
    }
}
