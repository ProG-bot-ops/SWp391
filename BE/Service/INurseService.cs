using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWP391_SE1914_ManageHospital.Service
    
{
    public interface INurseService
    {
        Task<NurseResponseDTO> GetNurseByIdAsync(int id);
        Task<IEnumerable<NurseResponseDTO>> GetAllNursesAsync();
        Task<IEnumerable<NurseResponseDTO>> GetNurseByNameAsync(string name);
        Task<NurseResponseDTO> CreateNurseAsync(NurseCreate nurseCreateDto);
        Task<NurseResponseDTO> UpdateNurseAsync(int id, NurseUpdate nurseUpdateDto);
        Task<bool> DeleteNurseAsync(int id, NurseDelete nurseDeleteDto);
        Task<NurseRegisterResponse> NurseRegisterAsync(NurseRegisterRequest request);
    }
}