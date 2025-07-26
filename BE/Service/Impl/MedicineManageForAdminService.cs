using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicineManageForAdminService : IMedicineManageForAdminService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineManageForAdminMapper _mapper;


        public MedicineManageForAdminService(ApplicationDBContext context, IMedicineManageForAdminMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<MedicineManageForAdminDTO>> GetAllInfoMedicineInventoryAsync()
        {
            var data = await _context.Medicines
            .Include(m => m.MedicineCategory)
            .Include(m => m.Unit)
            .Include(m => m.MedicineImportDetails!)
                .ThenInclude(d => d.Import)
                    .ThenInclude(i => i.Supplier)
            .Include(m => m.MedicineDetail)
            .ToListAsync();

            return _mapper.ListEntityToInventoryDTO(data).ToList();

        }
    }
}
