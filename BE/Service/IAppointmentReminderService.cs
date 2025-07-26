using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IAppointmentReminderService
{
    Task<List<Appointment>> SendEmailReminder();
}
