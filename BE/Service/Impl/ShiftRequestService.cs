using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System;

namespace SWP391_SE1914_ManageHospital.Models.Services.Impl
{
    public class ShiftRequestService : IShiftRequestService
    {
        private readonly ApplicationDBContext _context;

        public ShiftRequestService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ShiftRequestResponseDTO> CreateAsync(ShiftRequestRequestDTO dto)
        {
            var shift = await _context.Doctor_Shifts.FindAsync(dto.ShiftId);
            if (shift == null)
                throw new ArgumentException("Ca trực không tồn tại.");

            var isDuplicate = await _context.ShiftRequests
                .AnyAsync(r => r.DoctorId == dto.DoctorId && r.ShiftId == dto.ShiftId && r.Status == "Pending");
            if (isDuplicate)
                throw new InvalidOperationException("Bạn đã gửi yêu cầu cho ca trực này và đang chờ duyệt.");

            var entity = new ShiftRequest
            {
                DoctorId = dto.DoctorId,
                ShiftId = dto.ShiftId,
                RequestType = dto.RequestType,
                Reason = dto.Reason,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };
            _context.ShiftRequests.Add(entity);
            await _context.SaveChangesAsync();

            await _context.Entry(entity).Reference(e => e.Doctor).LoadAsync();
            await _context.Entry(entity).Reference(e => e.Shift).LoadAsync();

            return new ShiftRequestResponseDTO
            {
                Id = entity.Id,
                DoctorId = entity.DoctorId,
                ShiftId = entity.ShiftId,
                RequestType = entity.RequestType,
                Reason = entity.Reason,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
                ApprovedDate = entity.ApprovedDate,
                DoctorName = entity.Doctor?.Name,
                ShiftType = entity.Shift?.ShiftType,
                ShiftDate = entity.Shift?.ShiftDate
            };
        }

        public async Task<IEnumerable<ShiftRequestResponseDTO>> GetByDoctorAsync(int doctorId)
        {
            var requests = await _context.ShiftRequests
                .Include(r => r.Doctor)
                .Include(r => r.Shift)
                .Where(r => r.DoctorId == doctorId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return requests.Select(r => new ShiftRequestResponseDTO
            {
                Id = r.Id,
                DoctorId = r.DoctorId,
                ShiftId = r.ShiftId,
                RequestType = r.RequestType,
                Reason = r.Reason,
                Status = r.Status,
                CreatedDate = r.CreatedDate,
                ApprovedDate = r.ApprovedDate,
                DoctorName = r.Doctor?.Name,
                ShiftType = r.Shift?.ShiftType,
                ShiftDate = r.Shift?.ShiftDate
            });
        }

        public async Task<IEnumerable<ShiftRequestResponseDTO>> GetAllAsync()
        {
            var requests = await _context.ShiftRequests
                .Include(r => r.Doctor)
                .Include(r => r.Shift)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return requests.Select(r => new ShiftRequestResponseDTO
            {
                Id = r.Id,
                DoctorId = r.DoctorId,
                ShiftId = r.ShiftId,
                RequestType = r.RequestType,
                Reason = r.Reason,
                Status = r.Status,
                CreatedDate = r.CreatedDate,
                ApprovedDate = r.ApprovedDate,
                DoctorName = r.Doctor?.Name,
                ShiftType = r.Shift?.ShiftType,
                ShiftDate = r.Shift?.ShiftDate
            });
        }

        public async Task<bool> ApproveAsync(int requestId)
        {
            var request = await _context.ShiftRequests.FindAsync(requestId);
            if (request == null)
                throw new ArgumentException("Yêu cầu không tồn tại.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Yêu cầu đã được xử lý.");

            request.Status = "Approved";
            request.ApprovedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAsync(int requestId, string? reason = null)
        {
            var request = await _context.ShiftRequests.FindAsync(requestId);
            if (request == null)
                throw new ArgumentException("Yêu cầu không tồn tại.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Yêu cầu đã được xử lý.");

            request.Status = "Rejected";
            request.ApprovedDate = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(reason))
                request.Reason += $"\n[Lý do từ chối]: {reason}";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}