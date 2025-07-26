using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Feedback;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

public class FeedbackService : IFeedbackService
{
    private readonly ApplicationDBContext _context;
    private readonly IFeedbackMapper _mapper;

    public FeedbackService(ApplicationDBContext context, IFeedbackMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FeedbackResponseDTO> CreateFeedbackAsync(FeedbackCreate create)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor_Appointments)
            .FirstOrDefaultAsync(a => a.Id == create.AppointmentId);

        if (appointment == null) 
            throw new Exception("Không tìm thấy lịch hẹn");

        if (appointment.Status != AppointmentStatus.Completed)
            throw new Exception("Chỉ có thể gửi sau khi hoàn tất khám");

        var existingFeedback = await _context.Feedbacks.
            FirstOrDefaultAsync(f => f.AppointmentId == appointment.Id);

        if (existingFeedback != null)
            throw new Exception("Bạn đã gửi nhận xét cho lịch khám này rồi");

        

        var feedback = _mapper.CreateToEntity(create);

        feedback.CreateDate = DateTime.Now;
        feedback.AppointmentId = appointment.Id;
        feedback.PatientId = appointment.PatientId;

        var doctorId = appointment.Doctor_Appointments.FirstOrDefault()?.DoctorId;
        if (doctorId != null)
        {
            feedback.DoctorId = doctorId;
        }
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        var response = _mapper.EntityToResponse(feedback);
        return response;

    }

    public async Task<IEnumerable<FeedbackResponseDTO>> FindFeedbackByPatientIdAsync(int id)
    {
        var coId = await _context.Feedbacks
            .Where(a => a.PatientId == id).ToListAsync();

        if (coId == null)
            throw new Exception("Không có Nhận xét nào của Bệnh nhân này");

        var response = _mapper.ListEntityToResponse(coId);
        return response;
    }

    public async Task<IEnumerable<FeedbackResponseDTO>> GetAllFeedbackAsync()
    {
        var coId = await _context.Feedbacks.ToListAsync();

        if (coId == null)
        {
            throw new Exception("Không có bản ghi nào");
        }

        var response = _mapper.ListEntityToResponse(coId);
        return response;
    }

    public async Task<bool> HardDeleteFeedbackAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<FeedbackResponseDTO>> SearchFeedbackByFilterAsync(string? name, DateTime? appointmentDate, DateTime? startTime)
    {
        var query = _context.Feedbacks.Include(a => a.Patient).Include(a => a.Appointment).AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.Patient.Name.Contains(name));
        }
        if (appointmentDate.HasValue)
        {
            query = query.Where(a => a.Appointment.AppointmentDate.Date == appointmentDate.Value.Date);
        }
        if (startTime.HasValue)
        {
            query = query.Where(a => a.Appointment.StartTime == startTime.Value.TimeOfDay);
        }

        var feedbackList = await query.ToListAsync();

        if (feedbackList == null || feedbackList.Count == 0)
            throw new Exception("Không có bản ghi feedback nào");

        var response = _mapper.ListEntityToResponse(feedbackList);
        return response;
    }
}
