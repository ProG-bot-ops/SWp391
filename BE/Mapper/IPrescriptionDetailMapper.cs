// Mapper/IPrescriptionDetailMapper.cs
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PrescriptionDetail;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IPrescriptionDetailMapper
    {
        PrescriptionDetailResponseDTO MapToResponse(PrescriptionDetail entity);
        PrescriptionDetail MapToEntity(PrescriptionDetailRequest request, string userName);
        void MapToExistingEntity(PrescriptionDetailRequest request, PrescriptionDetail entity, string userName);
    }
}
