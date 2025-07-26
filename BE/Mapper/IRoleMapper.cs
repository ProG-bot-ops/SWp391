
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Role;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper;

public interface IRoleMapper
{
    // request => Entity(DTO)
    Role CreateToEntity(RoleCreate create);
    Role UpdateToEntity(RoleUpdate update);
    Role DeleteToEntity(RoleDelete delete);

    // Entity(DTO) => Response
    RoleResponseDTO EntityToResponse(Role entity);
    IEnumerable<RoleResponseDTO> ListEntityToResponse(IEnumerable<Role> entities);
}
