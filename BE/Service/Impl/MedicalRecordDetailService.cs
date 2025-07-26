using System;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicalRecord;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicalRecordDetailService : IMedicalRecordDetailService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicalRecordDetailMapper _detailMapper;

        public MedicalRecordDetailService(
            ApplicationDBContext context,
            IMedicalRecordDetailMapper detailMapper)
        {
            _context = context;
            _detailMapper = detailMapper;
        }

        public MedicalRecordDetailResponse? GetMedicalRecordDetail(int medicalRecordId)
        {
            try
            {
                if (medicalRecordId <= 0)
                {
                    throw new ArgumentException("ID Medical Record không hợp lệ", nameof(medicalRecordId));
                }

                var record = _context.Medical_Records
                    .Include(mr => mr.Doctor)
                    .Include(mr => mr.Patient)
                    .Include(mr => mr.Disease)
                    .FirstOrDefault(mr => mr.Id == medicalRecordId);

                if (record == null)
                {
                    return null;
                }

                return _detailMapper.EntityToDetailResponse(record);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết Medical Record ID: {medicalRecordId}", ex);
            }
        }

        public MedicalRecordDetailResponse CreateMedicalRecord(MedicalRecordCreateRequest request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Thông tin Medical Record không được để trống");
                }

                if (string.IsNullOrWhiteSpace(request.Diagnosis))
                {
                    throw new ArgumentException("Chẩn đoán không được để trống", nameof(request));
                }

                ValidateRelatedEntities(request.DoctorId, request.PatientId, request.DiseaseId);

                var entity = _detailMapper.CreateRequestToEntity(request);
                _context.Medical_Records.Add(entity);
                _context.SaveChanges();

                _context.Entry(entity).Reference(e => e.Doctor).Load();
                _context.Entry(entity).Reference(e => e.Patient).Load();
                _context.Entry(entity).Reference(e => e.Disease).Load();

                return _detailMapper.EntityToDetailResponse(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo mới Medical Record", ex);
            }
        }

        public MedicalRecordDetailResponse UpdateMedicalRecord(int id, MedicalRecordUpdateRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("ID Medical Record không hợp lệ", nameof(id));
                }

                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Thông tin cập nhật không được để trống");
                }

                if (string.IsNullOrWhiteSpace(request.Diagnosis))
                {
                    throw new ArgumentException("Chẩn đoán không được để trống", nameof(request));
                }

                ValidateRelatedEntities(request.DoctorId, request.PatientId, request.DiseaseId);

                var entity = _context.Medical_Records
                    .Include(mr => mr.Doctor)
                    .Include(mr => mr.Patient)
                    .Include(mr => mr.Disease)
                    .FirstOrDefault(mr => mr.Id == id);

                if (entity == null)
                {
                    throw new ArgumentException($"Không tìm thấy Medical Record với ID: {id}", nameof(id));
                }

                _detailMapper.UpdateEntityFromRequest(entity, request);
                _context.SaveChanges();

                _context.Entry(entity).Reference(e => e.Doctor).Load();
                _context.Entry(entity).Reference(e => e.Patient).Load();
                _context.Entry(entity).Reference(e => e.Disease).Load();

                return _detailMapper.EntityToDetailResponse(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật Medical Record ID: {id}", ex);
            }
        }

        public bool DeleteMedicalRecord(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("ID Medical Record không hợp lệ", nameof(id));
                }

                var entity = _context.Medical_Records.FirstOrDefault(mr => mr.Id == id);
                if (entity == null)
                {
                    return false;
                }

                _context.Medical_Records.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xoá Medical Record ID: {id}", ex);
            }
        }

        private void ValidateRelatedEntities(int doctorId, int patientId, int? diseaseId)
        {
            // Kiểm tra bác sĩ tồn tại
            bool doctorExists = _context.Doctors.Any(d => d.Id == doctorId);
            if (!doctorExists)
            {
                throw new ArgumentException($"Không tìm thấy bác sĩ với ID: {doctorId}", nameof(doctorId));
            }

            // Kiểm tra bệnh nhân tồn tại
            bool patientExists = _context.Patients.Any(p => p.Id == patientId);
            if (!patientExists)
            {
                throw new ArgumentException($"Không tìm thấy bệnh nhân với ID: {patientId}", nameof(patientId));
            }

            // Kiểm tra bệnh tồn tại nếu có truyền vào
            if (diseaseId.HasValue)
            {
                bool diseaseExists = _context.Diseases.Any(d => d.Id == diseaseId.Value);
                if (!diseaseExists)
                {
                    throw new ArgumentException($"Không tìm thấy bệnh với ID: {diseaseId}", nameof(diseaseId));
                }
            }
        }
    }
}