using Microsoft.AspNetCore.Http.HttpResults;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl;

public class ClinicMapper : IClinicMapper
{
    public Clinic CreateToEntity(ClinicCreate create)
    {
        Clinic clinic = new Clinic();
        clinic.Name = create.Name;
        clinic.Code = create.Code;
        clinic.Status = create.Status;
        clinic.CreateDate = create.CreateDate;
        clinic.UpdateDate = create.UpdateDate;
        clinic.CreateBy = create.CreateBy;
        clinic.UpdateBy = create.UpdateBy;
        return clinic;
    }

    public Clinic DeleteToEntity(ClinicDelete delete)
    {
        Clinic clinic = new Clinic();
        clinic.Id = delete.Id;
        clinic.Name = delete.Name;
        clinic.Code = delete.Code;
        clinic.Status = delete.Status;
        clinic.CreateDate = delete.CreateDate;
        clinic.UpdateDate = delete.UpdateDate;
        clinic.CreateBy = delete.CreateBy;
        clinic.UpdateBy = delete.UpdateBy;
        return clinic;
    }

    public ClinicResponseDTO EntityToResponse(Clinic entity)
    {
        ClinicResponseDTO response = new ClinicResponseDTO();
        response.Id = entity.Id;
        response.Code = entity.Code;
        response.Name = entity.Name;
        response.Status = entity.Status;
        response.CreateDate = entity.CreateDate;
        response.CreateBy = entity.CreateBy;
        response.UpdateDate = entity.UpdateDate;
        response.UpdateBy = entity.UpdateBy;
        response.Address = entity.Address;
        response.Email = entity.Email;
        response.ImageUrl = entity.ImageUrl;
        return response;
    }

    public IEnumerable<ClinicResponseDTO> ListEntityToResponse(IEnumerable<Clinic> entities)
    {
        return entities.Select(x => EntityToResponse(x)).ToList();
    }

    public Clinic UpdateToEntity(ClinicUpdate update)
    {
        Clinic clinic = new Clinic();
        clinic.Id = update.Id;
        clinic.Name = update.Name;
        clinic.Code = update.Code;
        clinic.Status = update.Status;
        clinic.CreateDate = update.CreateDate;
        clinic.UpdateDate = update.UpdateDate;
        clinic.CreateBy = update.CreateBy;
        clinic.UpdateBy = update.UpdateBy;
        return clinic;
    }
}
