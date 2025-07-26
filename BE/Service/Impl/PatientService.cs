using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using SWP391_SE1914_ManageHospital.Ultility.Validation;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDBContext _context;
        private readonly IpatientMapper _mapper;

        public PatientService(ApplicationDBContext context, IpatientMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> CheckUniqueCodeAsync()
        {
            string newCode;
            do
            {
                newCode = "PAT" + Guid.NewGuid().ToString("N")[..5].ToUpper();
            } while (await _context.Patients.AnyAsync(p => p.Code == newCode));
            return newCode;
        }

        public async Task<PatientRespone> CreatePatientAsync(PatientCreate create)
        {
            if (create.UserId > 0)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == create.UserId);
                if (!userExists)
                {
                    throw new Exception($"User with Id {create.UserId} does not exist.");
                }
            }
            Patient entity = _mapper.CreateToEntity(create);

            await _context.Patients.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.EntityToRespone(entity);
        }


        public async Task<PatientRespone> FindPatientByIdAsync(int id)
        {
            var patient =  await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new Exception($"Can not find patient id: {id}");
            }
            return _mapper.EntityToRespone(patient);
        }

        public async Task<PatientRespone> FindPatientByUserIdAsync(int id)
        {
            var coID = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == id)
                ?? throw new Exception($"Không thể tìm Bệnh nhân có user id: {id}");
            return _mapper.EntityToRespone(coID);
        }

        public async Task<IEnumerable<PatientRespone>> GetAllPatientAsync()
        {
            var patients = await _context.Patients.ToListAsync();
            if (patients == null)
            {
                throw new Exception("No Patient");
            }

            return _mapper.ListEntityToRespone(patients);
        }

        public async Task<bool> HardDeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new Exception($"Can not find patient ID: {id}");
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        

        public async Task<IEnumerable<PatientInfoAdmin>> PatientInfoAdAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Medical_Record)
                    .ThenInclude(mr => mr.Doctor)
                .Include(a => a.Invoice)
                .ToListAsync();

            if (!appointments.Any())
            {
                throw new Exception("Không tìm thấy lịch sử khám của bệnh nhân.");
            }

            var result = _mapper.PatientInfoAdmins(appointments);
            return result;
        }


        public async Task<IEnumerable<PatientRespone>> SearchPatientByKeyAsync(string key)
        {
            var result = await _context.Patients
                .Where(p => p.Name.Contains(key))
                .ToListAsync();

            if (!result.Any())
            {
                throw new Exception($"Can not find patient with key: \"{key}\".");
            }

            return _mapper.ListEntityToRespone(result);
        }

        public async Task<PatientRespone> SoftDeletePatientColorAsync(int id, Status.PatientStatus newStatus)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new Exception($"Can not fint patient with ID: {id}");
            }

            patient.Status = newStatus;
            patient.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return _mapper.EntityToRespone(patient);
        }

        public async Task<PatientRespone> UpdatePatientAsync(int id, PatientUpdate update)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new Exception($"Can not fint patient with ID: {id}");
            }

            
            patient.Name = update.Name;
            patient.Gender = update.Gender;
            patient.Dob = update.Dob;
            patient.CCCD = update.CCCD;
            patient.Phone = update.Phone;
            patient.EmergencyContact = update.EmergencyContact;
            patient.Address = update.Address;
            patient.InsuranceNumber = update.InsuranceNumber;
            patient.Allergies = update.Allergies;
            patient.BloodType = update.BloodType;
            patient.ImageURL = update.ImageURL;
            patient.Status = update.Status;
            patient.UpdateBy = update.UpdateBy;
            patient.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return _mapper.EntityToRespone(patient);
        }

        public async Task<PatientRespone> UpdatePatientByUserIdAsync(int userId, PatientUpdate update)
        {
            var coID = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId)
                ?? throw new Exception($"Không thể tìm Bệnh nhân có user id: {userId}");

            PatientUpdateValidator.Validate(update);

            coID.Name = update.Name.Trim();
            coID.Gender = update.Gender;
            coID.Dob = update.Dob;
            coID.Code = update.Code;
            coID.CCCD = update.CCCD;
            coID.Phone = update.Phone;
            coID.EmergencyContact = update.EmergencyContact;
            coID.Address = update.Address;
            coID.InsuranceNumber = update.InsuranceNumber;
            coID.Allergies = update.Allergies;
            coID.BloodType = update.BloodType;
            coID.ImageURL = update.ImageURL;
            coID.Status = update.Status;
            coID.UpdateBy = update.Name;
            coID.UpdateDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return _mapper.EntityToRespone(coID);

        }

        public async Task<PatientRespone> UpdatePatientImageAsync(int userId, PatientImageUpdate imageurl)
        {
            var coId = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId)
                ?? throw new Exception($"Không thể tìm Bệnh nhân có user id: {userId}");
            coId.ImageURL = imageurl.ImageURL;
            await _context.SaveChangesAsync();
            return _mapper.EntityToRespone(coId);
        }
    }
}
