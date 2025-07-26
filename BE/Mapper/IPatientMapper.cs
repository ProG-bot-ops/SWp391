using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public interface IpatientMapper 
    {
        // request => Entity(DTO)
        Patient CreateToEntity(PatientCreate create);
        Patient DeleteToEntity(PatientDelete delete);
        Patient UpdateToEntity(PatientUpdate update);

        // Entity(DTO) => Response
        PatientRespone EntityToRespone(Patient entity);
        IEnumerable<PatientRespone> ListEntityToRespone (IEnumerable<Patient> entities);
        IEnumerable<PatientInfoAdmin> PatientInfoAdmins (IEnumerable<Appointment> appointments);
    }
}
