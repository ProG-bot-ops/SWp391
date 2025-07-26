using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service
{
    public class AppointmentReminderService : BackgroundService
    {
        private readonly ILogger<AppointmentReminderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Kiểm tra mỗi phút
        private readonly TimeSpan _reminderTime = new TimeSpan(5, 0, 0); // 5:00 AM

        public AppointmentReminderService(
            ILogger<AppointmentReminderService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Appointment Reminder Service đã khởi động.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendReminderEmails();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong Appointment Reminder Service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Đợi 5 phút nếu có lỗi
                }
            }
        }

        private async Task SendReminderEmails()
        {
            var now = DateTime.Now;
            var today = now.Date;

            // Chỉ gửi email nhắc nhở vào lúc 5:00 AM
            if (now.TimeOfDay < _reminderTime || now.TimeOfDay > _reminderTime.Add(TimeSpan.FromMinutes(1)))
            {
                return;
            }

            _logger.LogInformation("Bắt đầu gửi email nhắc nhở cho ngày {Date}", today);

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                // Lấy tất cả lịch hẹn cho ngày hôm nay với status phù hợp
                var appointments = await context.Appointments
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Doctor_Appointments)
                        .ThenInclude(da => da.Doctor)
                    .Include(a => a.Clinic)
                    .Where(a => a.AppointmentDate.Date == today &&
                               a.Status != AppointmentStatus.InProgress && // Không gửi cho lịch hẹn đang khám
                               a.Status != AppointmentStatus.Cancelled) // Không gửi cho lịch hẹn đã hủy
                    .ToListAsync();

                _logger.LogInformation("Tìm thấy {Count} lịch hẹn cần gửi nhắc nhở", appointments.Count);

                foreach (var appointment in appointments)
                {
                    try
                    {
                        var userEmail = appointment.Patient?.User?.Email;
                        if (string.IsNullOrEmpty(userEmail))
                        {
                            _logger.LogWarning("Không tìm thấy email cho lịch hẹn {AppointmentId}", appointment.Id);
                            continue;
                        }

                        // Lấy tên doctor từ Doctor_Appointments
                        var doctorName = appointment.Doctor_Appointments
                            .FirstOrDefault()?.Doctor?.Name ?? "";

                        await emailService.SendAppointmentReminderEmailAsync(
                            toEmail: userEmail,
                            patientName: appointment.Patient?.Name ?? "Bệnh nhân",
                            appointmentCode: appointment.Code ?? "",
                            appointmentDate: appointment.AppointmentDate,
                            shift: appointment.Shift ?? "",
                            doctorName: doctorName,
                            clinicName: appointment.Clinic?.Name ?? ""
                        );

                        _logger.LogInformation("Đã gửi email nhắc nhở cho lịch hẹn {AppointmentCode} đến {Email}", 
                            appointment.Code, userEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi gửi email nhắc nhở cho lịch hẹn {AppointmentId}", appointment.Id);
                    }
                }

                _logger.LogInformation("Hoàn thành gửi email nhắc nhở cho ngày {Date}", today);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý email nhắc nhở cho ngày {Date}", today);
            }
        }
    }
} 