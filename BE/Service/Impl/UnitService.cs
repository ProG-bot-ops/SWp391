using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Unit;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class UnitService : IUnitService
    {
        private readonly ApplicationDBContext _context;
        private readonly IUnitMapper _mapper;

        public UnitService(ApplicationDBContext context, IUnitMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UnitResponseDTO> CreateUnitAsync(UnitCreate create)
        {
            if (await _context.Units.AnyAsync(x => x.Name == create.Name)) 
            {
                throw new Exception("Tên đã được sử dụng");
            }

            Unit entity = _mapper.CreateToEntity(create);
            await _context.Units.AddAsync(entity);
            await _context.SaveChangesAsync();
            var response = _mapper.EntityToResponse(entity);
            return response;
        }

        public async Task<IEnumerable<UnitResponseDTO>> GetAll()
        {
            // Lấy danh sách tất cả đơn vị
            var units = await _context.Units.ToListAsync();

            // Kiểm tra nếu danh sách rỗng
            if (units.Count == 0)
            {
                throw new Exception("Không có bản ghi nào");
            }

            // Chuyển đổi dữ liệu từ entity sang DTO
            var response = _mapper.ListEntityToResponse(units);

            return response;
        }


        public async Task<IEnumerable<UnitResponseDTO>> SearchName(string name)
        {
            
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Tên không được để trống");
            }

            
            name = name.ToLower();

           
            var units = await _context.Units
                .Where(u => u.Name.ToLower().Contains(name))  
                .ToListAsync();

            if (units == null || units.Count == 0)
            {
                throw new Exception($"Không có đơn vị nào với tên chứa \"{name}\".");
            }

            
            var response = _mapper.ListEntityToResponse(units);
            return response;
        }

       

        public async Task<UnitResponseDTO> UpdateUnitAsync(UnitUpdate update)
        {
            try
            {
                
                var unit = await _context.Units
                    .FirstOrDefaultAsync(u => u.Id == update.Id);

                
                if (unit == null)
                {
                    throw new Exception($"Không tìm thấy đơn vị với ID {update.Id}");
                }

                
                if (await _context.Units.AnyAsync(x => x.Name == update.Name && x.Id != update.Id))
                {
                    throw new Exception("Tên đơn vị đã tồn tại.");
                }

                
                unit.Name = update.Name;
                unit.Status = update.Status;

                
                _context.Units.Update(unit);
                await _context.SaveChangesAsync();

               
                var response = _mapper.EntityToResponse(unit);
                return response;
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Lỗi khi cập nhật đơn vị: {ex.Message}");
                throw new Exception("Lỗi khi cập nhật đơn vị.");
            }
        }

    }
}
