using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Doctor;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System.Collections.Generic;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IDoctorMapper
    {
        DoctorResponseDTO MapToDto(Doctor doctor);
        IEnumerable<DoctorResponseDTO> MapToDtoList(IEnumerable<Doctor> doctors);
        Doctor MapToEntity(DoctorCreate doctorCreateDto);
        void MapToEntity(DoctorUpdate doctorUpdateDto, Doctor doctor);
    }
}