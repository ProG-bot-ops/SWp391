using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Department;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper;

public interface IDepartmentMapper
{
    // request => Entity(DTO)
    Department CreateToEntity(DepartmentCreate create);
    Department UpdateToEntity(DepartmentUpdate update);
    Department DeleteToEntity(DepartmentDelete delete);

    // Entity(DTO) => Response
    DepartmentResponseDTO EntityToResponse(Department entity);
    IEnumerable<DepartmentResponseDTO> ListEntityToResponse(IEnumerable<Department> entities);
}
