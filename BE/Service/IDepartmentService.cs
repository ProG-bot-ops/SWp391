using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Department;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IDepartmentService
{
    public Task<IEnumerable<DepartmentResponseDTO>> GetAllDepartmentAsync();
    public Task<IEnumerable<DepartmentResponseDTO>> SearchDepartmentByKeyAsync(string name);
    public Task<DepartmentResponseDTO> UpdateDepartmentAsync(int id, DepartmentUpdate update);
    public Task<DepartmentResponseDTO> CreateDepartmentAsync(DepartmentCreate create);
    public Task<DepartmentResponseDTO> SoftDeleteDepartmentAsync(int id, DepartmentStatus newStatus);
    public Task<bool> HardDeleteDepartmentAsync(int id);
    public Task<DepartmentResponseDTO> FindDepartmentByIdAsync(int id);
    public Task<string> CheckUniqueCodeAsync();
}
