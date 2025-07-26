using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Service
{
    public class MedicineCategoryService : IMedicineCategoryService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineCategoryMapper _mapper;

        public MedicineCategoryService(ApplicationDBContext context, IMedicineCategoryMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MedicineCategoryResponseDTO>> GetAllAsync()
        {
            var list = await _context.MedicineCategories
                                     .AsNoTracking()
                                     .ToListAsync();
            return list.Select(m => _mapper.MapToDTO(m)).ToList();
        }

        public async Task<MedicineCategoryResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _context.MedicineCategories.FindAsync(id);
            return entity == null ? null : _mapper.MapToDTO(entity);
        }

        public async Task<MedicineCategoryResponseDTO> CreateAsync(MedicineCategoryRequest request)
        {
            var entity = _mapper.MapToEntity(request);
            _context.MedicineCategories.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.MapToDTO(entity);
        }

        public async Task<MedicineCategoryResponseDTO?> UpdateAsync(int id, MedicineCategoryRequest request)
        {
            var entity = await _context.MedicineCategories.FindAsync(id);
            if (entity == null) return null;

            _mapper.MapToExistingEntity(request, entity);
            await _context.SaveChangesAsync();
            return _mapper.MapToDTO(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.MedicineCategories.FindAsync(id);
            if (entity == null) return false;

            _context.MedicineCategories.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
