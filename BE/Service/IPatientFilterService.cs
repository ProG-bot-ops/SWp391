using System.Collections.Generic;
using System.Threading.Tasks;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PatientFilter;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IPatientFilterService
    {
        /// Lấy danh sách bệnh nhân của bác sĩ dựa theo bộ lọc
        Task<List<PatientFilterResponse>> GetPatientsByDoctorAsync(PatientFilter filter);

        /// Lấy danh sách bệnh nhân của bác sĩ trong ngày hiện tại
        Task<List<PatientFilterResponse>> GetTodayPatientsByDoctorAsync(int doctorId);
    }
}