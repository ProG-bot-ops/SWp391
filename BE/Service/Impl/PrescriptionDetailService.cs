// Service/Impl/PrescriptionDetailService.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PrescriptionDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class PrescriptionDetailService : IPrescriptionDetailService
    {
        private readonly ApplicationDBContext _context;
        private readonly IPrescriptionDetailMapper _mapper;

        public PrescriptionDetailService(ApplicationDBContext context, IPrescriptionDetailMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PrescriptionDetailResponseDTO>> GetAllAsync()
        {
            var list = await _context.PrescriptionDetails
                .AsNoTracking()
                .ToListAsync();
            return list.Select(_mapper.MapToResponse).ToList();
        }

        public async Task<PrescriptionDetailResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _context.PrescriptionDetails.FindAsync(id);
            return entity == null ? null : _mapper.MapToResponse(entity);
        }



        public async Task<List<PrescriptionDetailResponseDTO>> GetByPrescriptionIdAsync(int prescriptionId)
        {
            // 1. Lấy danh sách detail kèm Medicine
            var details = await _context.PrescriptionDetails
                .Where(d => d.PrescriptionId == prescriptionId)
                .Include(d => d.Medicine)        // để có d.Medicine.Name, d.Medicine.Code
                .AsNoTracking()
                .ToListAsync();

            // 2. Tập hợp medicineId duy nhất
            var meds = details.Select(d => d.MedicineId).Distinct().ToList();

            // 3. Build map medicineId -> unitPrice (lấy lô InStock sớm nhất)
            var priceMap = new Dictionary<int, decimal>();
            foreach (var mid in meds)
            {
                var price = await _context.Medicine_Inventories
                    .Where(mi => mi.MedicineId == mid
                              && mi.Quantity > 0
                              && mi.Status == (int)MedicineInventoryStatus.InStock)
                    .OrderBy(mi => mi.ExpiryDate)
                    .Select(mi => mi.UnitPrice)
                    .FirstOrDefaultAsync();
                priceMap[mid] = price;
            }

            // 4. Map ra DTO
            var result = details.Select(d =>
            {
                var up = priceMap.TryGetValue(d.MedicineId, out var p) ? p : 0m;
                return new PrescriptionDetailResponseDTO
                {
                    Id = d.Id,
                    PrescriptionId = d.PrescriptionId,
                    MedicineId = d.MedicineId,
                    MedicineName = d.Medicine.Name,
                    Quantity = d.Quantity,
                    Usage = d.Usage,
                    Status = d.Status.ToString(),
                    CreateDate = d.CreateDate,
                    UpdateDate = d.UpdateDate!.Value,
                    CreateBy = d.CreateBy,
                    UpdateBy = d.UpdateBy,
                    UnitPrice = up,
                    Total = up * d.Quantity
                };
            }).ToList();

            return result;
        }



        public async Task<PrescriptionDetailResponseDTO> CreateAsync(
    PrescriptionDetailRequest req,
    int userId
)
        {
            // Kiểm tra số lượng thuốc
            if (req.Quantity <= 0)
                throw new InvalidOperationException("Số lượng thuốc phải lớn hơn 0.");

            // 1. Tìm Prescription
            var prescription = await _context.Prescriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == req.PrescriptionId);
            if (prescription == null)
                throw new InvalidOperationException("Không tìm thấy đơn thuốc.");

            // Check trạng thái đơn thuốc
            if (prescription.Status != PrescriptionStatus.New)
                throw new InvalidOperationException("Chỉ có thể thêm chi tiết cho đơn thuốc mới.");

            // 2. Tìm Medicine
            var medicine = await _context.Medicines
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Name == req.MedicineName);
            if (medicine == null)
                throw new InvalidOperationException($"Không tìm thấy thuốc {req.MedicineName}");

            // 3. Lấy list inventory FIFO
            var inventories = await _context.Medicine_Inventories
                .Where(mi => mi.MedicineId == medicine.Id
                          && mi.Quantity > 0
                          && mi.Status == 0)
                .OrderBy(mi => mi.ExpiryDate)
                .ToListAsync();

            var totalAvailable = inventories.Sum(mi => mi.Quantity);
            if (totalAvailable < req.Quantity)
                throw new InvalidOperationException(
                    $"Không đủ thuốc trong kho. Cần {req.Quantity}, còn {totalAvailable}."
                );

            // 4. Tìm bác sĩ để gán CreateBy/UpdateBy
            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null)
                throw new InvalidOperationException("Không tìm thấy bác sĩ.");

            // 5. Tạo detail
            var detail = new PrescriptionDetail
            {
                PrescriptionId = req.PrescriptionId,
                MedicineId = medicine.Id,
                Quantity = req.Quantity,
                Usage = req.Usage,
                Status = (PrescriptionDetailStatus)req.Status,
                Name = medicine.Name,
                Code = medicine.Code,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                CreateBy = doctor.Name!,
                UpdateBy = doctor.Name!
            };

            _context.PrescriptionDetails.Add(detail);
            await _context.SaveChangesAsync();

            return _mapper.MapToResponse(detail);
        }





        public async Task<bool> DeleteAsync(int id)
        {
            var detail = await _context.PrescriptionDetails.FindAsync(id);
            if (detail == null) return false;
            _context.PrescriptionDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<PrescriptionDetailResponseDTO>> GetByUserAsync(int userId, string role)
        {
            // 1. Lấy toàn bộ detail kèm Prescription → Doctor/Patient
            IQueryable<PrescriptionDetail> query = _context.PrescriptionDetails
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Doctor)
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .Include(d => d.Medicine);  // có thể include Medicine để lấy tên

            if (role.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
                query = query.Where(d => d.Prescription.Doctor.UserId == userId);
            else if (role.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                query = query.Where(d => d.Prescription.Patient.UserId == userId);

            var list = await query.AsNoTracking().ToListAsync();

            // 2. Tập hợp medicineId duy nhất để không query lặp
            var meds = list.Select(d => d.MedicineId).Distinct().ToList();

            // 3. Lấy price FIFO cho mỗi medicineId
            var priceMap = new Dictionary<int, decimal>();
            foreach (var mid in meds)
            {
                var price = await _context.Medicine_Inventories
                    .Where(mi => mi.MedicineId == mid
                              && mi.Quantity > 0
                              && mi.Status == (int)MedicineInventoryStatus.InStock /*0 nếu bạn dùng int*/)
                    .OrderBy(mi => mi.ExpiryDate)
                    .Select(mi => mi.UnitPrice)
                    .FirstOrDefaultAsync();

                priceMap[mid] = price;
            }

            // 4. Map ra DTO
            var result = list.Select(d =>
            {
                var up = priceMap.GetValueOrDefault(d.MedicineId, 0m);
                return new PrescriptionDetailResponseDTO
                {
                    Id = d.Id,
                    PrescriptionId = d.PrescriptionId,
                    MedicineId = d.MedicineId,
                    MedicineName = d.Medicine.Name,
                    Quantity = d.Quantity,
                    Usage = d.Usage,
                    Status = d.Status.ToString(),
                    CreateDate = d.CreateDate,
                    UpdateDate = d.UpdateDate!.Value,
                    CreateBy = d.CreateBy,
                    UpdateBy = d.UpdateBy,
                    UnitPrice = up,
                    Total = up * d.Quantity
                };
            });

            return result;
        }
    }
}
