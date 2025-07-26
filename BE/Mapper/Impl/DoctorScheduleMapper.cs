using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.DoctorShift;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class DoctorScheduleMapper : IDoctorScheduleMapper
    {
        public Doctor_Shift ToEntity(DoctorScheduleCreate dto)
        {
            return new Doctor_Shift
            {
                DoctorId = dto.DoctorId,
                ShiftDate = dto.ShiftDate,
                ShiftType = dto.ShiftType,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Notes = dto.Notes,
                CreateBy = dto.CreateBy,
                CreateDate = DateTime.UtcNow
            };
        }

        public void UpdateEntity(Doctor_Shift entity, DoctorScheduleUpdate dto)
        {
            entity.ShiftDate = dto.ShiftDate;
            entity.ShiftType = dto.ShiftType;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Notes = dto.Notes;
            entity.UpdateBy = dto.UpdateBy;
            entity.UpdateDate = DateTime.UtcNow;
        }

        public DoctorScheduleResponseDTO ToResponseDTO(Doctor_Shift entity, bool success = true, string? message = null, string? deleteBy = null)
        {
            return new DoctorScheduleResponseDTO
            {
                Id = entity.Id,
                DoctorId = entity.DoctorId,
                ShiftDate = entity.ShiftDate,
                ShiftType = entity.ShiftType,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Notes = entity.Notes,
                CreateBy = entity.CreateBy,
                UpdateBy = entity.UpdateBy,
                DeleteBy = deleteBy,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Success = success,
                Message = message
            };
        }
    }
}