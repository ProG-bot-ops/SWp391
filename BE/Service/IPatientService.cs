using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
namespace SWP391_SE1914_ManageHospital.Service
{
    public interface IPatientService
    {
        public Task<IEnumerable<PatientRespone>> GetAllPatientAsync();
        public Task<IEnumerable<PatientRespone>> SearchPatientByKeyAsync(string key);
        public Task<PatientRespone> UpdatePatientAsync(int id, PatientUpdate update);
        public Task<PatientRespone> UpdatePatientByUserIdAsync(int userId, PatientUpdate update);
        public Task<PatientRespone> UpdatePatientImageAsync(int userId, PatientImageUpdate imageurl);
        public Task<PatientRespone> CreatePatientAsync(PatientCreate create);
        public Task<PatientRespone> SoftDeletePatientColorAsync(int id, PatientStatus newStatus);
        public Task<bool> HardDeletePatientAsync(int id);
        public Task<PatientRespone> FindPatientByIdAsync(int id);
        public Task<PatientRespone> FindPatientByUserIdAsync(int id);
        public Task<string> CheckUniqueCodeAsync();
        public Task<IEnumerable<PatientInfoAdmin>> PatientInfoAdAsync();
    }
}
