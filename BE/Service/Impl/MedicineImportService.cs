using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImport;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicineImportService : IMedicineImportService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineImportMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MedicineImportService(ApplicationDBContext context, IMedicineImportMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "unknown";
        }
        public async Task<string> CheckUniqueCodeAsync()
        {
            string newCode;
            bool isExist;

            do
            {
                newCode = GenerateCode.GenerateClinicCode();
                _context.ChangeTracker.Clear();
                isExist = await _context.Suppliers.AnyAsync(p => p.Code == newCode);
            }
            while (isExist);

            return newCode;
        }

        public async Task<MedicineImportResponseDTO> CreateMedicineImportAsync(MedicineImportCreate create)
        {
            if (await _context.MedicineImports.AnyAsync(x => x.Name.Trim().ToLower() == create.Name.ToLower()))
            {
                throw new Exception("Tên đã được sử dụng!");
            }
            var MedicineImport = _mapper.CreateToEntity(create);
            if (!string.IsNullOrEmpty(create.Code) && create.Code != "string")
            {
                MedicineImport.Code = create.Code;
            }
            else
            {
                MedicineImport.Code = await CheckUniqueCodeAsync();
            }

            while (await _context.Clinics.AnyAsync(p => p.Code == MedicineImport.Code))
            {
                MedicineImport.Code = await CheckUniqueCodeAsync();
            }

            MedicineImport.CreateDate = DateTime.Now;
            MedicineImport.CreateBy = GetCurrentUserId();
            await _context.MedicineImports.AddAsync(MedicineImport);

            await _context.SaveChangesAsync();
            return _mapper.EntityToResponse(MedicineImport);
        }

        public async Task<IEnumerable<MedicineImportResponseDTO>> GetAllMedicineImportAsync()
        {
            var MedicineImport = await _context.MedicineImports.ToListAsync();
            return _mapper.ListEntityToResponse(MedicineImport);
        }

        public async Task<IEnumerable<MedicineImportResponseDTO>> SearchMedicineImportByName(string name)
        {
            var sn = await _context.MedicineImports.FromSqlRaw("Select * from medicine_imports where Name like {0}", "%" + name + "%").ToListAsync();
            if(sn == null)
            {
                throw new Exception($"Không có tên {name} nào tồn tại!");
            }
            var res = _mapper.ListEntityToResponse(sn);
            return res;
        }

        public async Task<MedicineImportResponseDTO> UpdateMedicineImportAsync(int id, MedicineImportUpdate update)
        {
            var ex = await _context.MedicineImports.FirstOrDefaultAsync(x => x.Id == id);
            if(ex == null)
            {
                throw new Exception("Không tìm thấy!");
            }
            ex.Name = update.Name;
            ex.Notes = update.Notes;
            ex.UpdateDate = DateTime.Now;
            ex.UpdateBy = GetCurrentUserId();

            _context.MedicineImports.Update(ex);
            await _context.SaveChangesAsync();
            return _mapper.EntityToResponse(ex);
        }

        public async Task<IEnumerable<MedicineResponseDTO>> GetByCategoryIdAsync(int categoryId)
        {
            var medicines = await _context.Medicines
                .Where(m => m.MedicineCategoryId == categoryId)
                .Select(m => new MedicineResponseDTO
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    MedicineCategoryName = m.MedicineCategory.Name,
                    UnitName = m.Unit.Name,
                    Status = m.Status
                    // thêm các trường khác nếu cần
                })
                .ToListAsync();

            return medicines;
        }

    }
}
