using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicalRecord;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IMedicalRecordDetailService
    {
        MedicalRecordDetailResponse? GetMedicalRecordDetail(int medicalRecordId);
        MedicalRecordDetailResponse CreateMedicalRecord(MedicalRecordCreateRequest request);
        MedicalRecordDetailResponse UpdateMedicalRecord(int id, MedicalRecordUpdateRequest request);
        bool DeleteMedicalRecord(int id);
    }
}