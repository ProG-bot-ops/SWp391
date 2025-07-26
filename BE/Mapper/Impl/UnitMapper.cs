using Microsoft.AspNetCore.Http.HttpResults;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Unit;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper.Impl
{
    public class UnitMapper : IUnitMapper
    {
        public Unit CreateToEntity(UnitCreate create)
        {
           Unit unit = new Unit();
            unit.Name = create.Name;
            unit.Status = create.Status;
            return unit;
        }

        public Unit CreateToEntity(UnitUpdate update)
        {
            Unit unit = new Unit();
            unit.Name = update.Name;
            unit.Status = update.Status;
            return unit;
        }

        public UnitResponseDTO EntityToResponse(Unit entity)
        {
            UnitResponseDTO response = new UnitResponseDTO();
            response.Id = entity.Id;
            response.Name = entity.Name;
            response.Status = entity.Status;
            return response;
        }

        public IEnumerable<UnitResponseDTO> ListEntityToResponse(IEnumerable<Unit> entities)
        {
            return entities.Select(x => EntityToResponse(x)).ToList();
        }
    }
}
