using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PatientFilter;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.Helps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SWP391_SE1914_ManageHospital.Service;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using Microsoft.Extensions.Logging;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class PatientFilterService : IPatientFilterService
    {
        private readonly ApplicationDBContext _context;
        private readonly IPatientFilterMapper _patientMapper;
        private readonly ILogger<PatientFilterService> _logger;

        public PatientFilterService(ApplicationDBContext context, IPatientFilterMapper patientMapper, ILogger<PatientFilterService> logger)
        {
            _context = context;
            _patientMapper = patientMapper;
            _logger = logger;
        }

        public async Task<List<PatientFilterResponse>> GetPatientsByDoctorAsync(PatientFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter), "Bộ lọc không được để trống.");

            if (filter.DoctorId <= 0)
                throw new ArgumentException("Thiếu thông tin DoctorId hoặc DoctorId không hợp lệ.", nameof(filter));

            _logger.LogInformation($"=== PatientFilterService.GetPatientsByDoctorAsync ===");
            _logger.LogInformation($"DoctorId: {filter.DoctorId}");
            _logger.LogInformation($"FromDate: {filter.FromDate}");
            _logger.LogInformation($"ToDate: {filter.ToDate}");
            _logger.LogInformation($"PatientName: {filter.PatientName}");

            // ✅ KIỂM TRA DOCTOR TỒN TẠI
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == filter.DoctorId);
            if (doctor == null)
            {
                _logger.LogWarning($"Doctor with ID {filter.DoctorId} not found");
                throw new ArgumentException($"Không tìm thấy bác sĩ có ID: {filter.DoctorId}", nameof(filter));
            }
            _logger.LogInformation($"Doctor found: {doctor.Name}");

            // ✅ KIỂM TRA DOCTOR_APPOINTMENTS
            var doctorAppointments = await _context.Doctor_Appointments
                .Where(da => da.DoctorId == filter.DoctorId)
                .ToListAsync();

            _logger.LogInformation($"Doctor_Appointments count: {doctorAppointments.Count}");

            if (doctorAppointments.Count == 0)
            {
                _logger.LogWarning("No Doctor_Appointments found for this doctor");
                return new List<PatientFilterResponse>();
            }

            // ✅ KIỂM TRA APPOINTMENTS
            var appointmentIds = doctorAppointments.Select(da => da.AppointmentId).ToList();
            _logger.LogInformation($"Appointment IDs: {string.Join(", ", appointmentIds)}");

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => appointmentIds.Contains(a.Id))
                .ToListAsync();

            _logger.LogInformation($"Appointments found: {appointments.Count}");

            foreach (var appointment in appointments)
            {
                _logger.LogInformation($"Appointment: ID={appointment.Id}, Patient={appointment.Patient?.Name ?? "null"}, Date={appointment.AppointmentDate:yyyy-MM-dd HH:mm}, Status={appointment.Status}");
            }

            // ✅ FILTER BY DATE
            if (filter.FromDate.HasValue)
            {
                var fromDate = filter.FromDate.Value.Date;
                _logger.LogInformation($"Filtering from date: {fromDate:yyyy-MM-dd}");

                var beforeFilter = appointments.Count;
                appointments = appointments.Where(a => a.AppointmentDate.Date >= fromDate).ToList();
                _logger.LogInformation($"After FromDate filter: {beforeFilter} -> {appointments.Count}");
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Date;
                _logger.LogInformation($"Filtering to date: {toDate:yyyy-MM-dd}");

                var beforeFilter = appointments.Count;
                appointments = appointments.Where(a => a.AppointmentDate.Date <= toDate).ToList();
                _logger.LogInformation($"After ToDate filter: {beforeFilter} -> {appointments.Count}");
            }

            // ✅ FILTER BY PATIENT NAME
            if (!string.IsNullOrEmpty(filter.PatientName))
            {
                var beforeFilter = appointments.Count;
                appointments = appointments.Where(a => a.Patient != null && a.Patient.Name.Contains(filter.PatientName)).ToList();
                _logger.LogInformation($"After PatientName filter: {beforeFilter} -> {appointments.Count}");
            }

            _logger.LogInformation($"Final appointments after all filters: {appointments.Count}");

            if (appointments.Count == 0)
            {
                _logger.LogWarning("No appointments found after filtering, returning empty list");
                return new List<PatientFilterResponse>();
            }

            // ✅ CONVERT TO PATIENTAPPOINTMENTDATA
            var patientDataList = appointments.Select(a => new PatientAppointmentData
            {
                Patient = a.Patient,
                ExaminationTime = a.AppointmentDate,
                AppointmentStatus = a.Status.ToString()
            }).ToList();

            _logger.LogInformation($"PatientDataList count: {patientDataList.Count}");

            // ✅ MAP TO RESPONSE
            var result = _patientMapper.ListEntityToResponse(patientDataList);
            _logger.LogInformation($"Mapped result count: {result.Count}");

            return result;
        }

        public async Task<List<PatientFilterResponse>> GetTodayPatientsByDoctorAsync(int doctorId)
        {
            if (doctorId <= 0)
            {
                throw new ArgumentException("ID bác sĩ không hợp lệ", nameof(doctorId));
            }

            var filter = new PatientFilter
            {
                DoctorId = doctorId,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddDays(1).AddTicks(-1)
            };

            return await GetPatientsByDoctorAsync(filter);
        }
    }
}