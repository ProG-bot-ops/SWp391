using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicineInventoryService : IMedicineInventoryService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineInventoryMapper _mapper;

        public MedicineInventoryService(ApplicationDBContext context, IMedicineInventoryMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MedicineInventoryPageDTO> GetPagedAsync(int pageNumber, int pageSize)
        {
            var totalItems = await _context.Medicine_Inventories.CountAsync();
            var toltalPage = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = await _context.Medicine_Inventories
                .Include(mid => mid.ImportDetail)
                    .ThenInclude(m => m.Medicine)
                        .ThenInclude(mc => mc.MedicineCategory)
                .Include(mid => mid.ImportDetail)
                    .ThenInclude(s => s.Supplier)
                .Include(mid => mid.ImportDetail)
                    .ThenInclude(u => u.Unit)
                .OrderBy(d => d.ExpiryDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = data.Select(d => new MedicineInventoryResponseDTO
            {
                Id = d.Id,
                Quantity = d.Quantity,
                MedicineCode = d.ImportDetail?.Medicine?.Code ?? string.Empty,
                BatchNumber = d.ImportDetail?.BatchNumber ?? string.Empty,
                MedicineName = d.ImportDetail?.Medicine?.Name ?? string.Empty,
                CategoryName = d.ImportDetail?.Medicine?.MedicineCategory?.Name ?? string.Empty,
                UnitName = d.ImportDetail?.Unit?.Name ?? string.Empty,
                Status = d.Status,
                ManufactureDate = d.ImportDetail?.ManufactureDate,
                ExpiryDate = d.ImportDetail?.ExpiryDate,
                SupplierName = d.ImportDetail?.Supplier?.Name ?? string.Empty,
                ImportDate = d.ImportDetail?.CreateDate
            }).ToList();

            return new MedicineInventoryPageDTO
            {
                Items = items,
                TotalPages = toltalPage
            };
        }

        public async Task<MedicineInventoryPageDTO> SearchAsync(string keyword, string sortBy, bool ascending = true, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword) && string.IsNullOrWhiteSpace(sortBy))
            {
                return new MedicineInventoryPageDTO
                {
                    Items = new List<MedicineInventoryResponseDTO>(),
                    TotalPages = 0
                };
            }

            keyword = keyword.Trim().ToLower();

            var query = _context.Medicine_Inventories
                .Include(mid => mid.ImportDetail)
                    .ThenInclude(m => m.Medicine)
                        .ThenInclude(mc => mc.MedicineCategory)
                .Include(mid => mid.ImportDetail)
                    .ThenInclude(s => s.Supplier)
                .Include(mid => mid.ImportDetail)
                    .ThenInclude(u => u.Unit)
                .AsQueryable();

            query = query.Where(d =>
                (d.ImportDetail != null && d.ImportDetail.Medicine != null && d.ImportDetail.Medicine.Name.ToLower().Contains(keyword)
                 || d.ImportDetail != null && d.ImportDetail.Medicine != null && d.ImportDetail.Medicine.MedicineCategory != null && d.ImportDetail.Medicine.MedicineCategory.Name.ToLower().Contains(keyword)
                 || d.ImportDetail != null && d.ImportDetail.Supplier != null && d.ImportDetail.Supplier.Name.ToLower().Contains(keyword))
                 );

            switch (sortBy)
            {
                case "medicineName":
                    query = ascending
                        ? query.OrderBy(d => d.ImportDetail != null && d.ImportDetail.Medicine != null ? d.ImportDetail.Medicine.Name : string.Empty)
                        : query.OrderByDescending(d => d.ImportDetail != null && d.ImportDetail.Medicine != null ? d.ImportDetail.Medicine.Name : string.Empty);
                    break;

                case "categoryName":
                    query = ascending
                        ? query.OrderBy(d => d.ImportDetail != null && d.ImportDetail.Medicine != null && d.ImportDetail.Medicine.MedicineCategory != null
                            ? d.ImportDetail.Medicine.MedicineCategory.Name : string.Empty)
                        : query.OrderByDescending(d => d.ImportDetail != null && d.ImportDetail.Medicine != null && d.ImportDetail.Medicine.MedicineCategory != null
                            ? d.ImportDetail.Medicine.MedicineCategory.Name : string.Empty);
                    break;

                case "unitName":
                    query = ascending
                        ? query.OrderBy(d => d.ImportDetail != null && d.ImportDetail.Unit != null ? d.ImportDetail.Unit.Name : string.Empty)
                        : query.OrderByDescending(d => d.ImportDetail != null && d.ImportDetail.Unit != null ? d.ImportDetail.Unit.Name : string.Empty);
                    break;

                case "quantity":
                    query = ascending
                        ? query.OrderBy(d => d.Quantity)
                        : query.OrderByDescending(d => d.Quantity);
                    break;

                case "expiryDate":
                    query = ascending
                        ? query.OrderBy(d => d.ImportDetail != null ? d.ImportDetail.ExpiryDate : DateTime.MinValue)
                        : query.OrderByDescending(d => d.ImportDetail != null ? d.ImportDetail.ExpiryDate : DateTime.MinValue);
                    break;

                case "supplierName":
                    query = ascending
                        ? query.OrderBy(d => d.ImportDetail != null && d.ImportDetail.Supplier != null ? d.ImportDetail.Supplier.Name : string.Empty)
                        : query.OrderByDescending(d => d.ImportDetail != null && d.ImportDetail.Supplier != null ? d.ImportDetail.Supplier.Name : string.Empty);
                    break;

                default:
                    break;
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);


            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = data.Select(d => new MedicineInventoryResponseDTO
            {
                Id = d.Id,
                Quantity = d.Quantity,
                MedicineCode = d.ImportDetail?.Medicine?.Code ?? string.Empty,
                BatchNumber = d.ImportDetail?.BatchNumber ?? string.Empty,
                MedicineName = d.ImportDetail?.Medicine?.Name ?? string.Empty,
                CategoryName = d.ImportDetail?.Medicine?.MedicineCategory?.Name ?? string.Empty,
                UnitName = d.ImportDetail?.Unit?.Name ?? string.Empty,
                Status = d.Status,
                ManufactureDate = d.ImportDetail?.ManufactureDate,
                ExpiryDate = d.ImportDetail?.ExpiryDate,
                SupplierName = d.ImportDetail?.Supplier?.Name ?? string.Empty,
                ImportDate = d.ImportDetail?.CreateDate
            }).ToList();

            return new MedicineInventoryPageDTO
            {
                Items = items,
                TotalPages = totalPages
            };
        }
    }
}
