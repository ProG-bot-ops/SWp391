using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Unit;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IUnitMapper
    {
        Unit CreateToEntity (UnitCreate ceate);
        Unit CreateToEntity(UnitUpdate update);

        UnitResponseDTO EntityToResponse(Unit entity);
        IEnumerable<UnitResponseDTO> ListEntityToResponse(IEnumerable<Unit> entities);
    }
}
