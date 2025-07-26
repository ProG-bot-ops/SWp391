using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class MedicalRecordListMapper : IMedicalRecordListMapper
    {
        public MedicalRecordResponse EntityToListResponse(Medical_Record entity)
        {
            return new MedicalRecordResponse
            {
                Id = entity.Id,
                Diagnosis = entity.Diagnosis,
                Status = entity.Status.ToString(),
                CreateDate = entity.CreateDate,
                DoctorName = entity.Doctor?.Name ?? "",
                PatientName = entity.Patient?.Name ?? "",
                DiseaseName = entity.Disease?.Name ?? ""
            };
        }

        public IEnumerable<MedicalRecordResponse> ListEntityToResponse(IEnumerable<Medical_Record> entities)
        {
            return entities.Select(EntityToListResponse);
        }
    }
}