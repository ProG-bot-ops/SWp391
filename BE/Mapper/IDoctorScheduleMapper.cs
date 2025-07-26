using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.DoctorShift;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IDoctorScheduleMapper
    {
        Doctor_Shift ToEntity(DoctorScheduleCreate dto);
        void UpdateEntity(Doctor_Shift entity, DoctorScheduleUpdate dto);
        DoctorScheduleResponseDTO ToResponseDTO(Doctor_Shift entity, bool success = true, string? message = null, string? deleteBy = null);
    }
}