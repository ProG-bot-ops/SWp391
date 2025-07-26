using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineImportDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Ultility;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicineImportDetailService : IMedicineImportDetailService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineImportDetailMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MedicineImportDetailService(ApplicationDBContext context, IMedicineImportDetailMapper mapper, IHttpContextAccessor httpContextAccessor)
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
                newCode = GenerateCode.GenerateMedicineImportDetailCode();
                _context.ChangeTracker.Clear();
                isExist = await _context.MedicineImportDetails.AnyAsync(p => p.Code == newCode);
            }
            while (isExist);

            return newCode;
        }

        public async Task<MedicineImportDetailResponseDTO> CreateMedicineImportDetail(MedicineImportDetailCreate create)
        {
            if (!string.IsNullOrEmpty(create.BatchNumber) && await _context.MedicineImportDetails.AnyAsync(m =>m.BatchNumber != null &&m.BatchNumber.ToLower() == create.BatchNumber.ToLower()))
            {
                throw new Exception("Batch Number used!");
            }
            var MID = _mapper.CreateToEntity(create);
            if (!string.IsNullOrEmpty(create.Code) && create.Code != "string")
            {
                MID.Code = create.Code;
            }
            else
            {
                MID.Code = await CheckUniqueCodeAsync();
            }

            while (await _context.Clinics.AnyAsync(p => p.Code == MID.Code))
            {
                MID.Code = await CheckUniqueCodeAsync();
            }

            MID.CreateDate = DateTime.Now;
            MID.CreateBy = GetCurrentUserId();
            await _context.MedicineImportDetails.AddAsync(MID);
            await _context.SaveChangesAsync();
            return _mapper.EntityToResponse(MID);
        }

        public async Task<MedicineImportDetailPageDTO> SearchMedicineImportDetailAsync(string keyword, DateTime? startDate, DateTime? endDate, string sortBy, bool ascending, int pageNumber, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(keyword) && !startDate.HasValue && !endDate.HasValue && string.IsNullOrEmpty(sortBy))
            {
                return new MedicineImportDetailPageDTO { Items = new List<MedicineImportDetailResponseDTO>(), TotalPages = 0 };
            }

            keyword = keyword.Trim().ToLower();

            var query = _context.MedicineImportDetails
                .Include(d => d.Medicine)
                .Include(d => d.Supplier)
                .Include(d => d.Unit)
                .Include(d => d.Medicine.MedicineCategory)
                .AsQueryable();

            query = query.Where(d => 
            (d.Medicine.Code != null && d.Medicine.Code.ToLower().Contains(keyword)) ||
            (d.Medicine.Name != null && d.Medicine.Name.ToLower().Contains(keyword)) ||
            (d.Medicine.MedicineCategory.Name != null && d.Medicine.MedicineCategory.Name.ToLower().Contains(keyword)) ||
            (d.Supplier.Name != null && d.Supplier.Name.ToLower().Contains(keyword))
            );

            if (startDate.HasValue)
            {
                query = query.Where(d => d.CreateDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(d => d.CreateDate <= endDate.Value);
            }
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy switch
                {
                    "medicineName" => ascending
                        ? query.OrderBy(d => d.Medicine.Name)
                        : query.OrderByDescending(d => d.Medicine.Name),

                    "categoryName" => ascending
                        ? query.OrderBy(d => d.Medicine.MedicineCategory.Name)
                        : query.OrderByDescending(d => d.Medicine.MedicineCategory.Name),

                    "unitName" => ascending
                        ? query.OrderBy(d => d.Unit.Name)
                        : query.OrderByDescending(d => d.Unit.Name),

                    "quantity" => ascending
                        ? query.OrderBy(d => d.Quantity)
                        : query.OrderByDescending(d => d.Quantity),

                    "expiryDate" => ascending
                        ? query.OrderBy(d => d.ExpiryDate)
                        : query.OrderByDescending(d => d.ExpiryDate),

                    "supplierName" => ascending
                        ? query.OrderBy(d => d.Supplier.Name)
                        : query.OrderByDescending(d => d.Supplier.Name),

                    _ => query.OrderByDescending(d => d.CreateDate) 
                };
            }
            else
            {
                query = query.OrderByDescending(d => d.CreateDate); 
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = await query.Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();

            var items = data.Select(d => new MedicineImportDetailResponseDTO
            {
                Id = d.Id,
                MedicineCode = d.Medicine?.Code ?? string.Empty,
                ImportId = d.ImportId,
                MedicineId = d.MedicineId,
                UnitId = d.UnitId,
                MedicineName = d.Medicine?.Name ?? "N/A",
                BatchNumber = d.BatchNumber,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                CategoryName = d.Medicine?.MedicineCategory?.Name ?? "N/A",
                ManufactureDate = d.ManufactureDate,
                ExpiryDate = d.ExpiryDate,
                SupplierName = d.Supplier?.Name ?? "N/A",
                UnitName = d.Unit?.Name ?? "N/A",
                CreateDate = d.CreateDate,
                CreateBy = d.CreateBy,
                UpdateBy = d.UpdateBy,
                UpdateDate = d.UpdateDate,
            }).ToList();

            return new MedicineImportDetailPageDTO
            {
                Items = items,
                TotalPages = totalPages
            };
        }

        public async Task<MedicineImportDetailResponseDTO> UpdateMedicineImportDetail(MedicineImportDetailUpdate update, int id)
        {
            var up = await _context.MedicineImportDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (up == null)
            {
                throw new Exception("Không tìm thấy!");
            }
            up.BatchNumber = update.BatchNumber;
            up.Quantity = update.Quantity;
            up.UnitPrice = update.UnitPrice;
            up.UnitId = update.UnitId;

            up.UpdateDate = DateTime.Now;
            up.UpdateBy = GetCurrentUserId();

            _context.MedicineImportDetails.Update(up);
            await _context.SaveChangesAsync();
            return _mapper.EntityToResponse(up);

        }

        public async Task<MedicineImportDetailPageDTO> GetMedicineImportDetailPageAsync(int pageNumber, int pageSize = 10)
        {
            
            var totalItems = await _context.MedicineImportDetails.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize); 

            
            var data = await _context.MedicineImportDetails
                .Include(d => d.Medicine)
                .Include(d => d.Supplier)
                .Include(d => d.Unit)
                .Include(d => d.Medicine.MedicineCategory)
                .OrderByDescending(d => d.CreateDate)
                .Skip((pageNumber - 1) * pageSize)  
                .Take(pageSize)                     
                .ToListAsync();

            var items = data.Select(d => new MedicineImportDetailResponseDTO
            {
                Id = d.Id,
                MedicineCode = d.Medicine?.Code ?? string.Empty,
                ImportId = d.ImportId,
                MedicineId = d.MedicineId,
                UnitId = d.UnitId,
                MedicineName = d.Medicine?.Name ?? "N/A",
                BatchNumber = d.BatchNumber,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                CategoryName = d.Medicine?.MedicineCategory?.Name ?? "N/A",
                ManufactureDate = d.ManufactureDate,
                ExpiryDate = d.ExpiryDate,
                SupplierName = d.Supplier?.Name ?? "N/A",
                UnitName = d.Unit?.Name ?? "N/A",
                CreateDate = d.CreateDate,
                CreateBy = d.CreateBy,
                UpdateBy = d.UpdateBy,
                UpdateDate = d.UpdateDate,
            }).ToList();

            return new MedicineImportDetailPageDTO
            {
                Items = items,
                TotalPages = totalPages
            };
        }
    }
}
