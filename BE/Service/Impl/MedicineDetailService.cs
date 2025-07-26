using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicineDetailService : IMedicineDetailService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineDetailMapper _mapper;

        public MedicineDetailService(ApplicationDBContext context, IMedicineDetailMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Lấy tất cả MedicineDetails
        public async Task<List<MedicineDetailResponseDTO>> GetAllAsync()
        {
            var details = await _context.MedicineDetails
                .Include(m => m.Medicine) // Liên kết với Medicine nếu cần
                .ToListAsync();

            return details.Select(d => _mapper.MapToResponse(d)).ToList();
        }

        // Lấy MedicineDetail theo ID
        public async Task<MedicineDetailResponseDTO> GetByIdAsync(int id)
        {
            var detail = await _context.MedicineDetails
                .Include(m => m.Medicine) // Bao gồm bảng Medicine nếu cần
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detail == null) return null;

            return _mapper.MapToResponse(detail);
        }

        // Tạo mới MedicineDetail
        public async Task<MedicineDetailResponseDTO> CreateAsync(MedicineDetailRequest request)
        {
            var detail = _mapper.MapToEntity(request);
            _context.MedicineDetails.Add(detail);
            await _context.SaveChangesAsync();

            return _mapper.MapToResponse(detail);
        }

        // Cập nhật MedicineDetail
        public async Task<MedicineDetailResponseDTO> UpdateAsync(int id, MedicineDetailRequest request)
        {
            var detail = await _context.MedicineDetails.FindAsync(id);
            if (detail == null) return null;

            _mapper.MapToExistingEntity(request, detail);
            await _context.SaveChangesAsync();

            return _mapper.MapToResponse(detail);
        }

        // Xóa MedicineDetail
        public async Task<bool> DeleteAsync(int id)
        {
            var detail = await _context.MedicineDetails.FindAsync(id);
            if (detail == null) return false;

            _context.MedicineDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return true;
        }

        // tìm thuốc theo thành phần 
        public async Task<List<MedicineDetailResponseDTO>> SearchByIngredientsAsync(string ingredients)
        {
            var details = await _context.MedicineDetails
                .Where(m => m.Ingredients.Contains(ingredients)) // Tìm kiếm theo thành phần
                .ToListAsync();

            return details.Select(d => new MedicineDetailResponseDTO
            {
                Id = d.Id,
                MedicineId = d.MedicineId,
                Ingredients = d.Ingredients,
                ExpiryDate = d.ExpiryDate,
                Manufacturer = d.Manufacturer,
                Warning = d.Warning,
                StorageInstructions = d.StorageInstructions,
                Status = (MedicineStatus)d.Status, // Ánh xạ int sang enum MedicineStatus
                CreateDate = d.CreateDate,
                UpdateDate = d.UpdateDate,
                CreateBy = d.CreateBy,
                UpdateBy = d.UpdateBy,
                Description = d.Description
            }).ToList();
        }

    }


}
