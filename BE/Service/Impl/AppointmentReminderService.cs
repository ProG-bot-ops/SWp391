
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl;


public class AppointmentReminderService : IAppointmentReminderService
{
    private readonly ApplicationDBContext _context;
    private readonly IEmailService _emailService;

    public AppointmentReminderService(ApplicationDBContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<List<Appointment>> SendEmailReminder()
    {
        
        var now = DateTime.Now; 
        var reminderThreshold = now.AddMinutes(30);
        Console.WriteLine(TimeZoneInfo.Local.DisplayName);
        var appointmentsToRemind = await _context.Appointments
            .Where(a =>
                !a.isSend &&
                a.Status == AppointmentStatus.Scheduled &&
                a.AppointmentDate.Date == reminderThreshold.Date &&
                a.StartTime.HasValue &&
                a.StartTime.Value.Hours == reminderThreshold.Hour &&
                a.StartTime.Value.Minutes == reminderThreshold.Minute)
            .Include(a => a.Patient)
                .ThenInclude(p => p.User)
            .ToListAsync();

        foreach (var appointment in appointmentsToRemind)
        {
            var email = appointment.Patient?.User?.Email;
            var name = appointment.Patient?.Name ?? "bạn";

            if (!string.IsNullOrEmpty(email))
            {
                await _emailService.SendAppointmentReminderEmailAsync(email, name, appointment.StartTime.HasValue ? appointment.AppointmentDate.Date.Add(appointment.StartTime.Value) : appointment.AppointmentDate.Date);
                appointment.isSend = true; 
            }
        }

        await _context.SaveChangesAsync();
        return appointmentsToRemind;
    }


}
