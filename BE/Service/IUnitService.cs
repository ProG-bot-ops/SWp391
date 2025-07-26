using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Unit;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IUnitService
    {
        public Task<IEnumerable<UnitResponseDTO>> GetAll();
        public Task<IEnumerable<UnitResponseDTO>> SearchName(string name);
        public Task<UnitResponseDTO> UpdateUnitAsync(UnitUpdate create);
        public Task<UnitResponseDTO> CreateUnitAsync(UnitCreate create);

    }
}
