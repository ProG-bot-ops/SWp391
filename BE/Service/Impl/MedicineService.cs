using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

public class MedicineService : IMedicineService
{
    private readonly ApplicationDBContext _context;
    private readonly IMedicineMapper _mapper;

    public MedicineService(ApplicationDBContext context, IMedicineMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<MedicineResponseDTO>> GetAllAsync()
    {
        var medicines = await _context.Medicines
            .Include(m => m.MedicineCategory)  // Bao gồm bảng MedicineCategory để lấy tên danh mục
            .Include(m => m.Unit)              // Bao gồm bảng Unit để lấy tên đơn vị
            .AsNoTracking()                   // Đảm bảo không theo dõi thay đổi (tăng hiệu suất)
            .ToListAsync();                   // Lấy dữ liệu từ cơ sở dữ liệu

        return medicines.Select(m => new MedicineResponseDTO
        {
            Id = m.Id,
            Name = m.Name,
            Code = m.Code,
            Dosage = m.Dosage,
            ImageUrl = m.ImageUrl,
            UnitId = m.UnitId,
            MedicineCategoryName = m.MedicineCategory.Name, // Truy xuất tên từ bảng MedicineCategory
            UnitName = m.Unit.Name,                         // Truy xuất tên từ bảng Unit
            Status = (MedicineStatus)m.Status,              // Chuyển từ int sang enum MedicineStatus
            UnitPrice = m.UnitPrice,                        // Giá thuốc
            MedicineCategoryId = m.MedicineCategoryId,     // ID của danh mục thuốc
            Prescribed = m.Prescribed.ToString(),          // Trạng thái prescribed
            CreateDate = m.CreateDate,                      // Ngày tạo
            UpdateDate = m.UpdateDate,                      // Ngày cập nhật
            CreateBy = m.CreateBy,                          // Người tạo
            UpdateBy = m.UpdateBy,                          // Người cập nhật
            Description = m.Description                     // Mô tả
        }).ToList(); // Ánh xạ từ entity sang DTO
    }


    public async Task<MedicineResponseDTO?> GetByIdAsync(int id)
    {
        // 1) Fetch thuốc kèm Category, Unit, Inventories → ImportDetail → Supplier
        var medicine = await _context.Medicines
            .Include(m => m.MedicineCategory)
            .Include(m => m.Unit)
            .Include(m => m.Medicine_Inventories)
                .ThenInclude(inv => inv.ImportDetail)
                    .ThenInclude(det => det.Supplier)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (medicine == null)
            return null;

        // 2) Lấy danh sách tên nhà cung cấp, sắp theo ImportDate tăng dần, loại bỏ trùng lặp
        var supplierNames = medicine.Medicine_Inventories
            .OrderBy(inv => inv.ImportDate)
            .Select(inv => inv.ImportDetail.Supplier.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .ToList();

        // 3) Gom thành chuỗi (nếu muốn) hoặc truyền thẳng list xuống DTO
        var supplierNamesString = supplierNames.Any()
            ? string.Join(", ", supplierNames)
            : null;

        // 4) Trả về DTO
        return new MedicineResponseDTO
        {
            Id = medicine.Id,
            Name = medicine.Name,
            Code = medicine.Code,
            Dosage = medicine.Dosage,
            ImageUrl = medicine.ImageUrl,
            UnitId = medicine.UnitId,
            MedicineCategoryName = medicine.MedicineCategory.Name,
            UnitName = medicine.Unit.Name,
            Status = medicine.Status,
            UnitPrice = medicine.UnitPrice,
            MedicineCategoryId = medicine.MedicineCategoryId,
            Prescribed = medicine.Prescribed.ToString(),
            CreateDate = medicine.CreateDate,
            UpdateDate = medicine.UpdateDate,
            CreateBy = medicine.CreateBy,
            UpdateBy = medicine.UpdateBy,
            Description = medicine.Description,

            // Nếu DTO chỉ có 1 string, bạn dùng trường này:
            SupplierName = supplierNamesString

            // Nếu bạn muốn DTO chứa list, đổi DTO thành:
            // public List<string> SupplierNames { get; set; }
            // rồi map:
            // SupplierNames = supplierNames
        };
    }




    public async Task<MedicineResponseDTO> CreateAsync(MedicineRequest request)
    {
        var entity = _mapper.MapToEntity(request);
        _context.Medicines.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.MapToDTO(entity);
    }

    public async Task<MedicineResponseDTO?> UpdateAsync(int id, MedicineRequest request)
    {
        var entity = await _context.Medicines.FindAsync(id);
        if (entity == null) return null;

        _mapper.MapToExistingEntity(request, entity);
        await _context.SaveChangesAsync();
        return _mapper.MapToDTO(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Medicines.FindAsync(id);
        if (entity == null) return false;

        _context.Medicines.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MedicineResponseDTO>> GetByPrescribedAsync(int prescribed)
    {
        var list = await _context.Medicines
            .Where(m => (int)m.Prescribed == prescribed)
            .AsNoTracking()
            .ToListAsync();

        return list.Select(_mapper.MapToDTO).ToList();
    }

    public async Task<IEnumerable<MedicineResponseDTO>> GetByCategoryIdAsync(int categoryId)
    {
        var medicines = await _context.Medicines
            .Include(m => m.MedicineCategory) // Bao gồm bảng MedicineCategory
            .Include(m => m.Unit) // Bao gồm bảng Unit 
            
            .Where(m => m.MedicineCategoryId == categoryId)
            .Select(m => new MedicineResponseDTO
            {
                Id = m.Id,
                Name = m.Name, // Lấy trực tiếp từ bảng Medicines
                Code = m.Code,
                ImageUrl = m.ImageUrl,
                UnitPrice = m.UnitPrice,
                Description =m.Description,
                MedicineCategoryName = m.MedicineCategory.Name, // Lấy tên từ bảng MedicineCategory
                UnitName = m.Unit.Name, // Lấy tên từ bảng Unit
                Status = (MedicineStatus)m.Status,  // Ánh xạ Status từ int sang enum MedicineStatus
            })
            .ToListAsync();

        return medicines;
    }




}
