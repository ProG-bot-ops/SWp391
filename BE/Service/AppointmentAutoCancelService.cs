using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Ultility;

namespace SWP391_SE1914_ManageHospital.Service
{
    public class AppointmentAutoCancelService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentAutoCancelService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Kiểm tra mỗi 5 phút

        public AppointmentAutoCancelService(
            IServiceProvider serviceProvider,
            ILogger<AppointmentAutoCancelService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AppointmentAutoCancelService đã khởi động");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCancelOverdueAppointments();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong AppointmentAutoCancelService");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Đợi 1 phút nếu có lỗi
                }
            }
        }

        private async Task CheckAndCancelOverdueAppointments()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            var now = DateTime.Now;
            var today = DateTime.Today;
            var cutoffTime = new DateTime(today.Year, today.Month, today.Day, 17, 0, 0); // 17:00

            // Chỉ chạy sau 17:00
            if (now < cutoffTime)
            {
                return;
            }

            // Lấy tất cả appointments trong ngày hôm nay với status "đã lên lịch" hoặc "đang khám"
            var overdueAppointments = await context.Appointments
                .Where(a => a.AppointmentDate.Date == today &&
                           (a.Status == Status.AppointmentStatus.Scheduled || 
                            a.Status == Status.AppointmentStatus.InProgress))
                .ToListAsync();

            if (!overdueAppointments.Any())
            {
                _logger.LogInformation("Không có lịch hẹn quá giờ cần hủy");
                return;
            }

            var cancelledCount = 0;
            foreach (var appointment in overdueAppointments)
            {
                appointment.Status = Status.AppointmentStatus.Cancelled;
                appointment.Note = $"Tự động hủy - Quá giờ khám (17:00) - {now:dd/MM/yyyy HH:mm}";
                cancelledCount++;

                _logger.LogInformation($"Đã hủy lịch hẹn ID: {appointment.Id}, Bệnh nhân: {appointment.Name}");
            }

            await context.SaveChangesAsync();

            _logger.LogInformation($"Đã tự động hủy {cancelledCount} lịch hẹn quá giờ");
        }
    }
} 