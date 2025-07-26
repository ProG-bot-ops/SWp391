using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Supplier;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using System.Text.RegularExpressions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDBContext _context;
        private readonly ISupplierMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SupplierService(ApplicationDBContext context, ISupplierMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        
        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "unknown";
        }


        public async Task<IEnumerable<SupplierResponeDTO>> GetAllSupplierAsync()
        {
            var suppliers = await _context.Suppliers.ToListAsync();
            return _mapper.ListEntityToResponse(suppliers);
        }

        public async Task<IEnumerable<SupplierResponeDTO>> SearchSupplierByKeyAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();


            var sid = await _context.Suppliers
                                     .Where(s => s.Name.ToLower().Contains(normalizedName)) 
                                     .ToListAsync();

            if (sid == null || !sid.Any())
            {
                throw new Exception($"Không có tên {name} nào tồn tại!");
            }

            var response = _mapper.ListEntityToResponse(sid);
            return response;
        }

        public async Task<SupplierResponeDTO> UpdateSupplerAsync(int id, SupplierUpdate update)
        {
            var existing = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null)
            {
                throw new Exception("Không tìm thấy nhà cung cấp!");
            }
            if (await _context.Suppliers.AnyAsync(x => x.Id != id && x.Code.Trim().ToLower() == update.Code.Trim().ToLower()))
            {
                throw new Exception("Mã nhà cung cấp đã tồn tại!");
            }
            if (await _context.Suppliers.AnyAsync(x => x.Id != id && x.Phone == update.Phone))
            {
                throw new Exception("Số điện thoại nhà cung cấp đã tồn tại!");
            }
            if (await _context.Suppliers.AnyAsync(x => x.Id != id && x.Email.Trim().ToLower() == update.Email.Trim().ToLower()))
            {
                throw new Exception("Email nhà cung cấp đã tồn tại!");
            }

            ValidateCode(update.Code);
            ValidatePhone(update.Phone);
            ValidateEmail(update.Email);

            existing.Name = update.Name;
            existing.Phone = update.Phone;
            existing.Email = update.Email;
            existing.Address = update.Address;
            existing.Code = update.Code;
            existing.UpdateDate = DateTime.Now;
            existing.UpdateBy = "system";
            

            _context.Suppliers.Update(existing);
            await _context.SaveChangesAsync();

            return _mapper.EntityToResponse(existing);
        }

        private void ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^0\d{9}$"))
            {
                throw new Exception("Số điện thoại phải có đúng 10 chữ số và bắt đầu bằng số 0.");
            }
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Trim().ToLower().EndsWith("@gmail.com"))
            {
                throw new Exception("Email phải kết thúc bằng @gmail.com.");
            }
        }

        private void ValidateCode(string code)
        {
            if (!string.IsNullOrWhiteSpace(code) && !Regex.IsMatch(code, @"^NCC\d+$"))
            {
                throw new Exception("Code phải bắt đầu bằng NCC theo định dạng NCC + số.");
            }
        }


        public async Task<SupplierResponeDTO> CreateSupplerAsync(SupplierCreate create)
        {
            ValidateCode(create.Code);
            ValidatePhone(create.Phone);
            ValidateEmail(create.Email);

            if (await _context.Suppliers.AnyAsync(x => x.Code.Trim().ToLower() == create.Code.Trim().ToLower()))
            {
                throw new Exception("Mã nhà cung cấp đã tồn tại!");
            }
            if (await _context.Suppliers.AnyAsync(x => x.Phone == create.Phone))
            {
                throw new Exception("Số điện thoại nhà cung cấp đã tồn tại!");
            }
            if (await _context.Suppliers.AnyAsync(x => x.Email.Trim().ToLower() == create.Email.Trim().ToLower()))
            {
                throw new Exception("Email nhà cung cấp đã tồn tại!");
            }
            var supplier = _mapper.CreateToEntity(create);
            if (!string.IsNullOrEmpty(create.Code) && create.Code != "string")
            {
                supplier.Code = create.Code;
            }
            else
            {
                supplier.Code = await CheckUniqueCodeAsync();
            }

            while (await _context.Clinics.AnyAsync(p => p.Code == supplier.Code))
            {
                supplier.Code = await CheckUniqueCodeAsync();
            }
            supplier.CreateDate = DateTime.Now;
            supplier.CreateBy = GetCurrentUserId();
            await _context.Suppliers.AddAsync(supplier);

            await _context.SaveChangesAsync();
            return _mapper.EntityToResponse(supplier);
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
    }
}
