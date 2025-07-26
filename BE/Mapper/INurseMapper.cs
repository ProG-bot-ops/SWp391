using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System.Collections.Generic;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface INurseMapper
    {
        NurseResponseDTO MapToDto(Nurse nurse);
        Nurse MapToEntity(NurseCreate nurseCreateDto);
        void MapToEntity(NurseUpdate nurseUpdateDto, Nurse nurse);
        IEnumerable<NurseResponseDTO> MapToDtoList(IEnumerable<Nurse> nurses);
    }
}