using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicalRecordListService : IMedicalRecordListService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicalRecordListMapper _listMapper;

        public MedicalRecordListService(
            ApplicationDBContext context,
            IMedicalRecordListMapper listMapper)
        {
            _context = context;
            _listMapper = listMapper;
        }

        public IEnumerable<MedicalRecordResponse> GetMedicalRecordsByPatientId(int patientId)
        {
            try
            {
                if (patientId <= 0)
                {
                    throw new ArgumentException("ID bệnh nhân không hợp lệ", nameof(patientId));
                }

                // Kiểm tra bệnh nhân có tồn tại không
                bool patientExists = _context.Patients.Any(p => p.Id == patientId);
                if (!patientExists)
                {
                    throw new ArgumentException($"Không tìm thấy bệnh nhân có ID: {patientId}", nameof(patientId));
                }

                var records = _context.Medical_Records
                    .Include(mr => mr.Doctor)
                    .Include(mr => mr.Patient)
                    .Include(mr => mr.Disease)
                    .Where(mr => mr.PatientId == patientId)
                    .OrderByDescending(mr => mr.CreateDate) // Sắp xếp theo ngày tạo mới nhất lên đầu
                    .ToList();

                return _listMapper.ListEntityToResponse(records);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách Medical Records", ex);
            }
        }
    }
}