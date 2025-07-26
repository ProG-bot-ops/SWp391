using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Service;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

public class ServiceService : IServiceService
{
    private readonly ApplicationDBContext _context;

    public ServiceService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceResponseDTO>> GetAllServicesAsync()
    {
        var services = await _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>()
            .Include(s => s.Department)
            .Select(s => new ServiceResponseDTO
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                Price = s.Price,
                Status = s.Status,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department != null ? s.Department.Name : null,
                CreateDate = s.CreateDate,
                UpdateDate = s.UpdateDate,
                CreateBy = s.CreateBy,
                UpdateBy = s.UpdateBy
            })
            .ToListAsync();

        return services;
    }

    public async Task<ServiceResponseDTO?> GetServiceByIdAsync(int id)
    {
        var service = await _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>()
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (service == null)
            return null;

        return new ServiceResponseDTO
        {
            Id = service.Id,
            Name = service.Name,
            Code = service.Code,
            Description = service.Description,
            ImageUrl = service.ImageUrl,
            Price = service.Price,
            Status = service.Status,
            DepartmentId = service.DepartmentId,
            DepartmentName = service.Department != null ? service.Department.Name : null,
            CreateDate = service.CreateDate,
            UpdateDate = service.UpdateDate,
            CreateBy = service.CreateBy,
            UpdateBy = service.UpdateBy
        };
    }

    public async Task<ServiceResponseDTO> CreateServiceAsync(ServiceRequestDTO request)
    {
        var service = new SWP391_SE1914_ManageHospital.Models.Entities.Service
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            Status = request.Status,
            DepartmentId = request.DepartmentId,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            CreateBy = "System", // This should come from authentication context
            UpdateBy = "System"  // This should come from authentication context
        };

        _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>().Add(service);
        await _context.SaveChangesAsync();

        return await GetServiceByIdAsync(service.Id) ?? new ServiceResponseDTO();
    }

    public async Task<ServiceResponseDTO?> UpdateServiceAsync(int id, ServiceRequestDTO request)
    {
        var service = await _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>().FindAsync(id);
        if (service == null)
            return null;

        service.Name = request.Name;
        service.Code = request.Code;
        service.Description = request.Description;
        service.ImageUrl = request.ImageUrl;
        service.Price = request.Price;
        service.Status = request.Status;
        service.DepartmentId = request.DepartmentId;
        service.UpdateDate = DateTime.Now;
        service.UpdateBy = "System"; // This should come from authentication context

        await _context.SaveChangesAsync();

        return await GetServiceByIdAsync(service.Id);
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var service = await _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>().FindAsync(id);
        if (service == null)
            return false;

        _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>().Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ServiceResponseDTO>> GetServicesByDepartmentAsync(int departmentId)
    {
        var services = await _context.Set<SWP391_SE1914_ManageHospital.Models.Entities.Service>()
            .Include(s => s.Department)
            .Where(s => s.DepartmentId == departmentId)
            .Select(s => new ServiceResponseDTO
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                Price = s.Price,
                Status = s.Status,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department != null ? s.Department.Name : null,
                CreateDate = s.CreateDate,
                UpdateDate = s.UpdateDate,
                CreateBy = s.CreateBy,
                UpdateBy = s.UpdateBy
            })
            .ToListAsync();

        return services;
    }
} 