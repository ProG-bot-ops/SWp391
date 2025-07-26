using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Role;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IRoleService
{
    public Task<IEnumerable<RoleResponseDTO>> GetAllRoleAsync();
    public Task<IEnumerable<RoleResponseDTO>> SearchRoleByKeyAsync(string name);
    public Task<RoleResponseDTO> UpdateRoleAsync(int id, RoleUpdate update);
    public Task<RoleResponseDTO> CreateRoleAsync(RoleCreate create);
    //public Task<RoleResponseDTO> SoftDeleteRoleAsync(int id, RoleStatus newStatus);
    public Task<bool> HardDeleteRoleAsync(int id);
    public Task<RoleResponseDTO> FindRoleByIdAsync(int id);
    public Task<string> CheckUniqueCodeAsync();
}
