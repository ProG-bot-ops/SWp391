using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Mapper;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class DoctorShiftFillerService : IDoctorShiftFillerService
    {
        private readonly ApplicationDBContext _context;
        private readonly IDoctorShiftFillerMapper _mapper;

        public DoctorShiftFillerService(ApplicationDBContext context, IDoctorShiftFillerMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Doctor_ShiftDTO>> GetDoctorShiftsAsync(DoctorShiftFilterRequestDTO filter)
        {
            // Kiểm tra DoctorId có hợp lệ không (nếu bắt buộc)
            if (filter.DoctorId > 0)
            {
                var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == filter.DoctorId);
                if (!doctorExists)
                    throw new ArgumentException("Bác sĩ không tồn tại.");
            }

            // Kiểm tra khoảng ngày hợp lệ (nếu có)
            if (filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate > filter.ToDate)
                throw new ArgumentException("Từ ngày phải nhỏ hơn hoặc bằng đến ngày.");

            var query = _context.Doctor_Shifts.Include(x => x.Doctor).AsQueryable();

            if (filter.DoctorId > 0)
                query = query.Where(x => x.DoctorId == filter.DoctorId);
            if (filter.FromDate.HasValue)
                query = query.Where(x => x.ShiftDate >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(x => x.ShiftDate <= filter.ToDate.Value);

            var entities = await query.OrderBy(x => x.ShiftDate).ToListAsync();
            return entities.Select(_mapper.ToDTO);
        }

        public async Task<Doctor_ShiftDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id ca trực không hợp lệ.");

            var entity = await _context.Doctor_Shifts
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy ca trực.");

            return _mapper.ToDTO(entity);
        }
    }
}