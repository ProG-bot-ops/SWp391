using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Clinic;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Ultility;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service.Impl;

public class ClinicService : IClinicService
{
    private readonly ApplicationDBContext _context;
    private readonly IClinicMapper _mapper;

    public ClinicService(ApplicationDBContext context, IClinicMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<string> CheckUniqueCodeAsync()
    {
        string newCode;
        bool isExist;

        do
        {
            newCode = GenerateCode.GenerateClinicCode();
            _context.ChangeTracker.Clear();
            isExist = await _context.Clinics.AnyAsync(p => p.Code == newCode);
        }
        while (isExist);

        return newCode;
    }

    public async Task<ClinicResponseDTO> CreateClinicAsync(ClinicCreate create)
    {
        if (await _context.Clinics.AnyAsync(x => x.Name == create.Name))
        {
            throw new Exception("Tên đã được sử dụng");
        }
        //requestDTO -> response
        Clinic entity =  _mapper.CreateToEntity(create);
        if (!string.IsNullOrEmpty(create.Code) && create.Code != "string")
        {
            entity.Code = create.Code;
        }
        else
        {
            entity.Code = await CheckUniqueCodeAsync();
        }

        while (await _context.Clinics.AnyAsync(p => p.Code == entity.Code))
        {
            entity.Code = await CheckUniqueCodeAsync();
        }
        await _context.Clinics.AddAsync(entity);
        await _context.SaveChangesAsync();
        var response = _mapper.EntityToResponse(entity);
        return response;
    }

    public async Task<ClinicResponseDTO> FindClinicByIdAsync(int id)
    {
        var cid = await _context.Clinics.FindAsync(id)
            ?? throw new Exception($"Không có Id {id} tồn tại!");
        
        var response = _mapper.EntityToResponse(cid);
        return response;
    }

    public async Task<IEnumerable<ClinicResponseDTO>> GetAllClinicAsync()
    {
        var cid = await _context.Clinics.ToListAsync();
        if(cid == null)
        {
            throw new Exception("Không có bản ghi nào");
        }
        var response = _mapper.ListEntityToResponse(cid);
        return response;
    }

    /*Người dùng requestDTO -> Controller -> Service -> Database
                                Database -> service -> Controller -> Répsonse
    */
    public async Task<bool> HardDeleteClinicAsync(int id)
    {
        var cid = await _context.Clinics.FindAsync(id)       
          ??  throw new KeyNotFoundException($"Không có Id {id} tồn tại!");
        
         _context.Clinics.Remove(cid);
        await _context.SaveChangesAsync();
        return true;
        
    }

    public async Task<ClinicResponseDTO> SoftDeleteClinicAsync(int id, Status.ClinicStatus newStatus)
    {
        var coId = await _context.Clinics.FindAsync(id)
            ?? throw new KeyNotFoundException($"Không có Id {id} tồn tại");

        coId.Status = newStatus;

        await _context.SaveChangesAsync();

        var response = _mapper.EntityToResponse(coId);
        return response;
    }

    public async Task<IEnumerable<ClinicResponseDTO>> SearchClinicByKeyAsync(string name)
    {
        var cid = await _context.Clinics.FromSqlRaw("Select * from Clinics where Name like {0}", "%" + name + "%").ToListAsync();
        if (cid == null || !cid.Any())
        {
            // Trả về danh sách rỗng thay vì throw exception
            return new List<ClinicResponseDTO>();
        }
        var response = _mapper.ListEntityToResponse(cid);
        return response;
    }

    public async Task<ClinicResponseDTO> UpdateClinicAsync(int id, ClinicUpdate update)
    {

        var coId = await _context.Clinics.FindAsync(id)
            ?? throw new KeyNotFoundException($"Không có ID {id} tồn tại");

        var result = _mapper.UpdateToEntity(update);
        coId.Code = result.Code;
        coId.Name = result.Name;
        coId.Status = result.Status;
        coId.CreateDate = result.CreateDate;
        coId.UpdateDate = result.UpdateDate;
        coId.CreateBy = result.CreateBy;
        coId.UpdateBy = result.UpdateBy;
        await _context.SaveChangesAsync();

        var response =  _mapper.EntityToResponse(coId);
        return response;
    }

    public async Task<IEnumerable<ClinicResponseDTO>> GetActiveClinicsForAppointmentAsync(DateTime date)
    {
        const int MAX_APPOINTMENTS_PER_DOCTOR_PER_DAY = 5;
        var targetDate = date.Date;

        var activeClinics = await _context.Clinics
            .Include(c => c.Doctors)
            .Where(c => c.Status == ClinicStatus.Available)
            .ToListAsync();

        var result = new List<ClinicResponseDTO>();

        foreach (var clinic in activeClinics)
        {
            bool allDoctorsFull = true;
            foreach (var doctor in clinic.Doctors)
            {
                var appointmentCount = await _context.Doctor_Appointments
                    .Where(da => da.DoctorId == doctor.Id &&
                                  da.Appointment.AppointmentDate.Date == targetDate)
                    .CountAsync();
                if (appointmentCount < MAX_APPOINTMENTS_PER_DOCTOR_PER_DAY)
                {
                    allDoctorsFull = false;
                    break;
                }
            }
            var dto = _mapper.EntityToResponse(clinic);
            dto.isFull = allDoctorsFull; // Thêm trạng thái kín lịch
            result.Add(dto);
        }
        return result;
    }
   
}
