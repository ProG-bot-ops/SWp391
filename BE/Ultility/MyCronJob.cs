using Quartz;

namespace SWP391_SE1914_ManageHospital.Ultility;

public class MyCronJob : IJob
{
    private readonly ILogger<MyCronJob> _logger;

    public MyCronJob(ILogger<MyCronJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running reminder job at {Time}", DateTime.Now);
        // Appointment reminder is now handled by Background Service
        await Task.CompletedTask;
    }
}
