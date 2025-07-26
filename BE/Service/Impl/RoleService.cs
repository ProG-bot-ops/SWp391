using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Role;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

public class RoleService : IRoleService
{
    private readonly ApplicationDBContext _context;
    private readonly IRoleMapper _mapper;

    public RoleService(ApplicationDBContext context, IRoleMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<string> CheckUniqueCodeAsync()
    {
        string newCode;
        bool isExist;

        do
        {
            newCode = GenerateCode.GenerateRolesCode();
            _context.ChangeTracker.Clear();
            isExist = await _context.Roles.AnyAsync(p => p.Code == newCode);
        }
        while (isExist);

        return newCode;
    }

    public async Task<RoleResponseDTO> CreateRoleAsync(RoleCreate create)
    {
        if (await _context.Roles.AnyAsync(x => x.Name == create.Name))
        {
            throw new Exception("Tên đã được sử dụng");
        }
        Role entity = _mapper.CreateToEntity(create);
        if (!string.IsNullOrEmpty(create.Code) && create.Code != "string")
        {
            entity.Code = create.Code;
        }
        else
        {
            entity.Code = await CheckUniqueCodeAsync();
        }

        while (await _context.Roles.AnyAsync(p => p.Code == entity.Code))
        {
            entity.Code = await CheckUniqueCodeAsync();
        }
        await _context.Roles.AddAsync(entity);
        await _context.SaveChangesAsync();
        var response = _mapper.EntityToResponse(entity);
        return response;
    }

    public async Task<RoleResponseDTO> FindRoleByIdAsync(int id)
    {
        var coId = await _context.Roles.FindAsync(id)
            ?? throw new KeyNotFoundException($"Không có Role nào chứa Id {id}");

        var response = _mapper.EntityToResponse(coId);
        return response;
    }

    public async Task<IEnumerable<RoleResponseDTO>> GetAllRoleAsync()
    {
        var coId = await _context.Roles.ToListAsync();
        if (coId == null) throw new Exception("Không có bản ghi nào");

        var response = _mapper.ListEntityToResponse(coId);
        return response;
    }

    public async Task<bool> HardDeleteRoleAsync(int id)
    {
        var coId = await _context.Roles.FindAsync(id)
            ?? throw new KeyNotFoundException($"Không có Role nào chứa Id {id}");

         _context.Remove(coId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<RoleResponseDTO>> SearchRoleByKeyAsync(string name)
    {
        var cid = await _context.Roles.FromSqlRaw("Select * from Roles where Name like {0}", "%" + name + "%").ToListAsync();
        if (cid == null)
        {
            throw new Exception($"Không có tên {name} nào tồn tại!");
        }
        var response = _mapper.ListEntityToResponse(cid);
        return response;
    }

    public async Task<RoleResponseDTO> UpdateRoleAsync(int id, RoleUpdate update)
    {
        var coId = await _context.Roles.FindAsync(id)
            ?? throw new KeyNotFoundException($"Không có ID {id} tồn tại");

        var result = _mapper.UpdateToEntity(update);
        coId.Code = result.Code;
        coId.Name = result.Name;
        
        coId.CreateDate = result.CreateDate;
        coId.UpdateDate = result.UpdateDate;
        coId.CreateBy = result.CreateBy;
        coId.UpdateBy = result.UpdateBy;
        await _context.SaveChangesAsync();

        var response = _mapper.EntityToResponse(coId);
        return response;
    }
}
