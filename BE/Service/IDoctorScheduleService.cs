using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.DoctorShift;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IDoctorScheduleService
    {
        Task<DoctorScheduleResponseDTO> CreateAsync(DoctorScheduleCreate dto);
        Task<DoctorScheduleResponseDTO> UpdateAsync(DoctorScheduleUpdate dto);
        Task<DoctorScheduleResponseDTO> DeleteAsync(DoctorScheduleDelete dto);
    }
}