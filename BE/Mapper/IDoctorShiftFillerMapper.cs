using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IDoctorShiftFillerMapper
    {
        Doctor_ShiftDTO ToDTO(Doctor_Shift entity);
        Doctor_Shift ToEntity(Doctor_ShiftDTO dto);
    }
}
