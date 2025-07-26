using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IDoctorShiftFillerService
    {
        Task<IEnumerable<Doctor_ShiftDTO>> GetDoctorShiftsAsync(DoctorShiftFilterRequestDTO filter);
        Task<Doctor_ShiftDTO?> GetByIdAsync(int id);
    }
}