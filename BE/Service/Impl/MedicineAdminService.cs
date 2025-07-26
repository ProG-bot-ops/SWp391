using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineAdmin;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System.Text.RegularExpressions;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class MedicineAdminService : IMedicineAdminService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMedicineAdminMapper _mapper;

        public MedicineAdminService(ApplicationDBContext context, IMedicineAdminMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MedicineAdminPage> GetMedicineAsync(int pageNumber, int pageSize)
        {
            var totalItems = await _context.Medicines.CountAsync(); 
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = await _context.Medicines
                .Include(d => d.MedicineDetail)
                .Include(d => d.Unit)
                .Include(d => d.MedicineCategory)
                .OrderBy(d => d.Id)
                .Skip((pageNumber -1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            

            var items = data.Select(d => new MedicineAdmin
            {

                Id = d.Id,
                MedicineImage = d.ImageUrl ?? string.Empty,
                MedicineCode = d.Code,
                MedicineName = d.Name,
                MedicineStatus = d.Status,  
                MDescription = d.Description ?? string.Empty,
                MDDescription = d.MedicineDetail?.Description ?? string.Empty,
                Prescribled = d.Prescribed,
                UnitName = d.Unit?.Name ?? string.Empty,
                UnitPrice = d.UnitPrice,
                CategoryName = d.MedicineCategory?.Name ?? string.Empty,
                Ingredients = d.MedicineDetail?.Ingredients ?? string.Empty,
                Warning = d.MedicineDetail?.Warning ?? string.Empty,
                StorageIntructions = d.MedicineDetail?.StorageInstructions ?? string.Empty,

            }).ToList();

            return new MedicineAdminPage
            {
                Items = items,
                TotalPages = totalPages,
            };

        }

        public async Task<MedicineAdminPage> Search(string keyword, decimal? startPrice, decimal? endPrice,int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(keyword) && !startPrice.HasValue && !endPrice.HasValue)
            {
                return new MedicineAdminPage { Items = new List<MedicineAdmin>(), TotalPages = 0 };
            }

            keyword = keyword.Trim().ToLower();
            var query = _context.Medicines
                .Include(d => d.MedicineDetail)
                .Include(d => d.Unit)
                .Include(d => d.MedicineCategory)
                .AsQueryable();

            query = query.Where(d =>
                (d.Code != null && d.Code.Trim().ToLower().Contains(keyword)) ||
                (d.Name != null && d.Name.Trim().ToLower().Contains(keyword)) ||
                (d.MedicineDetail.Ingredients != null && d.MedicineDetail.Ingredients.ToLower().Contains(keyword)) ||
                (d.MedicineCategory.Name != null && d.MedicineCategory.Name.ToLower().Contains(keyword))
            );

            if(startPrice.HasValue)
            {
                query = query.Where(d => d.UnitPrice >= startPrice.Value);
            }
            if (endPrice.HasValue) 
            {
                query = query.Where(d => d.UnitPrice <= endPrice.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Lấy dữ liệu theo trang
            var data = await query.Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            var items = data.Select(d => new MedicineAdmin
            {
                Id = d.Id,
                MedicineImage = d.ImageUrl ?? string.Empty,
                MedicineCode = d.Code,
                MedicineName = d.Name,
                MedicineStatus = d.Status,  
                MDescription = d.Description ?? string.Empty,
                MDDescription = d.MedicineDetail?.Description ?? string.Empty,
                Prescribled = d.Prescribed,
                UnitName = d.Unit?.Name ?? string.Empty,
                UnitPrice = d.UnitPrice,
                CategoryName = d.MedicineCategory?.Name ?? string.Empty,
                Ingredients = d.MedicineDetail?.Ingredients ?? string.Empty,
                Warning = d.MedicineDetail?.Warning ?? string.Empty,
                StorageIntructions = d.MedicineDetail?.StorageInstructions ?? string.Empty,
            }).ToList();

            return new MedicineAdminPage
            {
                Items = items,
                TotalPages = totalPages,
            };
        }


        public async Task<bool> UpdateMedicineAsync(int id, MedicineAdminUpdate update)
        {
            try
            {

                var medicine = await _context.Medicines
                    .Include(m => m.MedicineDetail)  
                    .Include(m => m.Unit)            
                    .Include(m => m.MedicineCategory) 
                    .Include(m => m.Medicine_Inventories)
                    .FirstOrDefaultAsync(m => m.Id == id);

               
                if (medicine == null)
                {
                    return false; 
                }

                medicine.Code = update.MedicineCode;
                medicine.Name = update.MedicineName;
                medicine.Status = update.Status;  
                medicine.Description = update.MDescription;
                medicine.Prescribed = update.Prescribed;
                medicine.UnitPrice = update.UnitPrice;
                
                if (medicine.MedicineDetail != null)
                {
                    medicine.MedicineDetail.Description = update.MDDescription;
                    medicine.MedicineDetail.Ingredients = update.Ingredients;
                    medicine.MedicineDetail.Warning = update.Warning;
                    medicine.MedicineDetail.StorageInstructions = update.StorageInstructions;
                }

                if (medicine.Medicine_Inventories != null && medicine.Medicine_Inventories.Any())
                {
                    foreach (var inventory in medicine.Medicine_Inventories)
                    {
                        if (medicine.Status == Ultility.Status.MedicineStatus.Discontinued)
                        {
                            inventory.Status = Ultility.Status.MedicineInventoryStatus.Discontinued;
                        }
                        else if (medicine.Status == Ultility.Status.MedicineStatus.Active)
                        {
                            inventory.Status = inventory.Quantity > 0
                                ? Ultility.Status.MedicineInventoryStatus.InStock
                                : Ultility.Status.MedicineInventoryStatus.OutOfStock;
                        }
                    }
                }


                medicine.UnitId = update.UnitId;
                medicine.MedicineCategoryId = update.MedicineCategoryId;

               
                _context.Medicines.Update(medicine); 
                await _context.SaveChangesAsync();  

                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Lỗi khi cập nhật thuốc: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMedicineImageAsync(int id, string imageUrl)
        {
            try
            {
                var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);

                if (medicine == null)
                {
                    return false; 
                }

                
                medicine.ImageUrl = imageUrl;

      
                _context.Medicines.Update(medicine);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Lỗi khi cập nhật ảnh thuốc: {ex.Message}");
                return false;
            }
        }

    }
}
