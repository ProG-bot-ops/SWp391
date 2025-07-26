using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Department;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl;

public class DepartmentMapper : IDepartmentMapper
{
    public Department CreateToEntity(DepartmentCreate create)
    {
        Department department = new Department();
        department.Code = create.Code;
        department.Name = create.Name;
        department.Description = create.Description;
        department.TotalAmountOfPeople = create.TotalAmountOfPeople;
        department.Status = create.Status;
        department.CreateDate = create.CreateDate;
        department.CreateBy = create.CreateBy;
        department.UpdateBy = create.UpdateBy;
        department.UpdateDate = create.UpdateDate;
        return department;
    }

    public Department DeleteToEntity(DepartmentDelete delete)
    {
        Department department = new Department();
        department.Code = delete.Code;
        department.Name = delete.Name;
        department.Description = delete.Description;
        department.Status = delete.Status;
        department.CreateDate = delete.CreateDate;
        department.UpdateDate = delete.UpdateDate;
        department.CreateBy = delete.CreateBy;
        department.UpdateBy = delete.UpdateBy;
        return department;
    }

    public DepartmentResponseDTO EntityToResponse(Department entity)
    {
        DepartmentResponseDTO response = new DepartmentResponseDTO();
        response.Id = entity.Id;
        response.Code = entity.Code;
        response.Name = entity.Name;
        response.Description = entity.Description;
        response.Status = entity.Status;
        response.TotalAmountOfPeople = entity.TotalAmountOfPeople;
        response.CreateDate = entity.CreateDate;
        response.UpdateDate = entity.UpdateDate;
        response.CreateBy = entity.CreateBy;
        response.UpdateBy = entity.UpdateBy;
        return response; 
    }

    public IEnumerable<DepartmentResponseDTO> ListEntityToResponse(IEnumerable<Department> entities)
    {
        return entities.Select(x => EntityToResponse(x)).ToList();
    }

    public Department UpdateToEntity(DepartmentUpdate update)
    {
        Department department = new Department();
        department.Code = update.Code;
        department.Name = update.Name;
        department.Description = update.Description;
        department.TotalAmountOfPeople = update.TotalAmountOfPeople;
        department.Status = update.Status;
        department.CreateDate = update.CreateDate;
        department.CreateBy = update.CreateBy;
        department.UpdateBy = update.UpdateBy;
        department.UpdateDate = update.UpdateDate;
        return department;
    }
}
