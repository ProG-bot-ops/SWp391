using Microsoft.AspNetCore.Http.HttpResults;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Department;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Role;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System.Data;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl;

public class RoleMapper : IRoleMapper
{
    public Role CreateToEntity(RoleCreate create)
    {
        var now = DateTime.UtcNow.AddHours(7);

        Role role = new Role();

        role.Name = create.Name;
        role.Code = create.Code;
        role.CreateDate = now;
        role.CreateBy = "Admin";
        role.UpdateDate = now;
        role.UpdateBy = "Admin";
        return role;
    }

    public Role DeleteToEntity(RoleDelete delete)
    {
        var now = DateTime.UtcNow.AddHours(7);

        Role role = new Role();
        role.Id = delete.Id;
        role.Name = delete.Name;
        role.Code = delete.Code;
        role.CreateDate = now;
        role.CreateBy = "Admin";
        role.UpdateDate = now;
        role.UpdateBy = "Admin";
        return role;
    }

    public RoleResponseDTO EntityToResponse(Role entity)
    {
        var now = DateTime.UtcNow.AddHours(7);
        RoleResponseDTO response = new RoleResponseDTO();

        response.Id = entity.Id;
        response.Code = entity.Code;
        response.Name = entity.Name;
        response.CreateDate = now;
        response.CreateBy = "Admin";
        response.UpdateDate = now;
        response.UpdateBy = "Admin";
        return response;

    }

    public IEnumerable<RoleResponseDTO> ListEntityToResponse(IEnumerable<Role> entities)
    {
        return entities.Select(x => EntityToResponse(x)).ToList();
    }

    public Role UpdateToEntity(RoleUpdate update)
    {
        var now = DateTime.UtcNow.AddHours(7);

        Role role = new Role();

        role.Name = update.Name;
        role.Code = update.Code;
        role.CreateDate = now;
        role.CreateBy = "Admin";
        role.UpdateDate = now;
        role.UpdateBy = "Admin";
        return role;
    }
}
