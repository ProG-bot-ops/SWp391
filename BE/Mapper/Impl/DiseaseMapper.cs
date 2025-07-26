using System;
using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Disease;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class DiseaseMapper : IDiseaseMapper
    {
        public DiseaseResponse MapToResponse(DiseaseDTO entity)
        {
            if (entity == null)
                return null;

            return new DiseaseResponse(
                entity.Id,
                entity.Name,
                entity.Code,
                entity.Description,
                entity.Status,
                entity.CreateDate,
                entity.UpdateDate,
                entity.CreateBy,
                entity.UpdateBy
            );
        }

        public DiseaseDTO MapToEntity(DiseaseCreateRequest request)
        {
            if (request == null)
                return null;

            return new DiseaseDTO
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                Status = request.Status,
                CreateDate = DateTime.Now,
                CreateBy = request.CreateBy
            };
        }

        public DiseaseDTO MapToEntity(DiseaseUpdateRequest request, DiseaseDTO existingEntity)
        {
            if (request == null || existingEntity == null)
                return existingEntity;

            existingEntity.Name = request.Name;
            existingEntity.Code = request.Code;
            existingEntity.Description = request.Description;
            existingEntity.Status = request.Status;
            existingEntity.UpdateDate = DateTime.Now;
            existingEntity.UpdateBy = request.UpdateBy;

            return existingEntity;
        }
    }
}