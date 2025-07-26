using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class DoctorShiftFillerMapper : IDoctorShiftFillerMapper
    {
        public Doctor_ShiftDTO ToDTO(Doctor_Shift entity)
        {
            return new Doctor_ShiftDTO
            {
                Id = entity.Id,
                DoctorId = entity.DoctorId,
                ShiftDate = entity.ShiftDate,
                ShiftType = entity.ShiftType,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Notes = entity.Notes,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CreateBy = entity.CreateBy,
                UpdateBy = entity.UpdateBy,
                DoctorName = entity.Doctor?.Name
            };
        }

        public Doctor_Shift ToEntity(Doctor_ShiftDTO dto)
        {
            return new Doctor_Shift
            {
                Id = dto.Id,
                DoctorId = dto.DoctorId,
                ShiftDate = dto.ShiftDate,
                ShiftType = dto.ShiftType,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Notes = dto.Notes,
                CreateDate = dto.CreateDate,
                UpdateDate = dto.UpdateDate,
                CreateBy = dto.CreateBy,
                UpdateBy = dto.UpdateBy
            };
        }
    }
}
