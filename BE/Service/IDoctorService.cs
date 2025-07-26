using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Doctor;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IDoctorService
    {
        Task<DoctorResponseDTO> GetDoctorByIdAsync(int id);
        Task<IEnumerable<DoctorResponseDTO>> GetAllDoctorsAsync();
        Task<IEnumerable<DoctorResponseDTO>> GetDoctorByNameAsync(string name);
        Task<DoctorResponseDTO> CreateDoctorAsync(DoctorCreate doctorCreateDto);
        Task<string> CheckUniqueCodeAsync();
        Task<DoctorResponseDTO> UpdateDoctorAsync(int id, DoctorUpdate doctorUpdateDto);
        Task<bool> DeleteDoctorAsync(int id, DoctorDelete doctorDeleteDto);
        Task<DoctorRegisterResponse> DoctorRegisterAsync(DoctorRegisterRequest request);
        Task<DoctorResponseDTO> GetDoctorByUserIdAsync(int userId);
        Task<IEnumerable<DoctorResponseDTO>> GetDoctorsByClinicIdAsync(int clinicId, DateTime date);
        Task<int?> GetDepartmentIdByDoctorIdAsync(int doctorId);
    }
}