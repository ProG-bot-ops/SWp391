using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.DoctorShift;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly ApplicationDBContext _context;
        private readonly IDoctorScheduleMapper _mapper;

        public DoctorScheduleService(ApplicationDBContext context, IDoctorScheduleMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DoctorScheduleResponseDTO> CreateAsync(DoctorScheduleCreate dto)
        {
            // Validation: DoctorId phải hợp lệ
            if (dto.DoctorId <= 0)
                return new DoctorScheduleResponseDTO { Success = false, Message = "DoctorId không hợp lệ." };

            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Bác sĩ không tồn tại." };

            // Validation: Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc
            if (dto.StartTime >= dto.EndTime)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc." };

            // Validation: Không được trùng ca trực cùng bác sĩ, cùng ngày, cùng loại ca
            var isDuplicate = await _context.Doctor_Shifts.AnyAsync(s =>
                s.DoctorId == dto.DoctorId &&
                s.ShiftDate.Date == dto.ShiftDate.Date &&
                s.ShiftType == dto.ShiftType);

            if (isDuplicate)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Bác sĩ đã có ca trực này trong ngày." };

            var entity = _mapper.ToEntity(dto);
            _context.Doctor_Shifts.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.ToResponseDTO(entity, true, "Tạo ca trực thành công.");
        }

        public async Task<DoctorScheduleResponseDTO> UpdateAsync(DoctorScheduleUpdate dto)
        {
            if (dto.Id <= 0)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Id ca trực không hợp lệ." };

            var entity = await _context.Doctor_Shifts.FindAsync(dto.Id);
            if (entity == null)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Không tìm thấy ca trực." };

            // Validation: Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc
            if (dto.StartTime >= dto.EndTime)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc." };

            // Validation: Không được trùng ca trực cùng bác sĩ, cùng ngày, cùng loại ca (trừ ca hiện tại)
            var isDuplicate = await _context.Doctor_Shifts.AnyAsync(s =>
                s.DoctorId == entity.DoctorId &&
                s.ShiftDate.Date == dto.ShiftDate.Date &&
                s.ShiftType == dto.ShiftType &&
                s.Id != dto.Id);

            if (isDuplicate)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Bác sĩ đã có ca trực này trong ngày." };

            _mapper.UpdateEntity(entity, dto);
            await _context.SaveChangesAsync();

            return _mapper.ToResponseDTO(entity, true, "Cập nhật ca trực thành công.");
        }

        public async Task<DoctorScheduleResponseDTO> DeleteAsync(DoctorScheduleDelete dto)
        {
            if (dto.Id <= 0)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Id ca trực không hợp lệ." };

            var entity = await _context.Doctor_Shifts.FindAsync(dto.Id);
            if (entity == null)
                return new DoctorScheduleResponseDTO { Success = false, Message = "Không tìm thấy ca trực." };

            _context.Doctor_Shifts.Remove(entity);
            await _context.SaveChangesAsync();

            return _mapper.ToResponseDTO(entity, true, "Xóa ca trực thành công.", dto.DeleteBy);
        }
    }
}