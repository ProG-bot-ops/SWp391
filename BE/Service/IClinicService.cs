using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Ultility;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service;

public interface IClinicService
{
    public Task<IEnumerable<ClinicResponseDTO>> GetAllClinicAsync();
    public Task<IEnumerable<ClinicResponseDTO>> SearchClinicByKeyAsync(string name);
    public Task<ClinicResponseDTO> UpdateClinicAsync(int id, ClinicUpdate update);
    public Task<ClinicResponseDTO> CreateClinicAsync(ClinicCreate create);
    public Task<ClinicResponseDTO> SoftDeleteClinicAsync(int id, ClinicStatus newStatus);
    public Task<bool> HardDeleteClinicAsync(int id);
    public Task<ClinicResponseDTO> FindClinicByIdAsync(int id);
    public Task<string> CheckUniqueCodeAsync();
    public Task<IEnumerable<ClinicResponseDTO>> GetActiveClinicsForAppointmentAsync(DateTime date);
}
