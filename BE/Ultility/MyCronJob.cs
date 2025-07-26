using Quartz;
using SWP391_SE1914_ManageHospital.Service;

namespace SWP391_SE1914_ManageHospital.Ultility;

public class MyCronJob : IJob
{
    private readonly ILogger<MyCronJob> _logger;
    private readonly IAppointmentReminderService _service;

    public MyCronJob(ILogger<MyCronJob> logger, IAppointmentReminderService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running reminder job at {Time}", DateTime.Now);
        var appointmentReminder = await _service.SendEmailReminder();
        
    }
}
