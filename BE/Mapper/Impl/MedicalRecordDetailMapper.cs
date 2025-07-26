using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicalRecord;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicalRecordDetailMapper : IMedicalRecordDetailMapper
    {
        public MedicalRecordDetailResponse EntityToDetailResponse(Medical_Record entity)
        {
            return new MedicalRecordDetailResponse
            {
                Id = entity.Id,
                Diagnosis = entity.Diagnosis,
                TestResults = entity.TestResults,
                Notes = entity.Notes,
                Status = entity.Status.ToString(),
                CreateDate = entity.CreateDate,
                DoctorName = entity.Doctor?.Name ?? "",
                PatientName = entity.Patient?.Name ?? "",
                DiseaseName = entity.Disease?.Name ?? "",
                AppointmentId = entity.AppointmentId,
                PrescriptionId = entity.PrescriptionId
            };
        }

        public Medical_Record CreateRequestToEntity(MedicalRecordCreateRequest request)
        {
            return new Medical_Record
            {
                Diagnosis = request.Diagnosis,
                TestResults = request.TestResults,
                Notes = request.Notes,
                Status = Enum.TryParse<MedicalRecordStatus>(request.Status, out var status) ? status : MedicalRecordStatus.Open,
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                DiseaseId = request.DiseaseId,
                AppointmentId = request.AppointmentId,
                PrescriptionId = request.PrescriptionId,
                CreateDate = DateTime.UtcNow
            };
        }

        public void UpdateEntityFromRequest(Medical_Record entity, MedicalRecordUpdateRequest request)
        {
            entity.Diagnosis = request.Diagnosis;
            entity.TestResults = request.TestResults;
            entity.Notes = request.Notes;
            entity.Status = Enum.TryParse<MedicalRecordStatus>(request.Status, out var status) ? status : entity.Status;
            entity.DoctorId = request.DoctorId;
            entity.PatientId = request.PatientId;
            entity.DiseaseId = request.DiseaseId;
            entity.AppointmentId = request.AppointmentId;
            entity.PrescriptionId = request.PrescriptionId;
        }
    }
}