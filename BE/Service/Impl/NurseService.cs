using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using SWP391_SE1914_ManageHospital.Service;
using SWP391_SE1914_ManageHospital.Ultility;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Service
{
    public class NurseService : INurseService
    {
        private readonly ApplicationDBContext _context;
        private readonly INurseMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public NurseService(ApplicationDBContext context, INurseMapper mapper, IPasswordHasher passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<NurseResponseDTO> GetNurseByIdAsync(int id)
        {
            var nurse = await _context.Nurses
                .Include(n => n.User)
                .Include(n => n.Department)
                .FirstOrDefaultAsync(n => n.Id == id);
            return _mapper.MapToDto(nurse);
        }

        public async Task<IEnumerable<NurseResponseDTO>> GetAllNursesAsync()
        {
            var nurses = await _context.Nurses
                .Include(n => n.User)
                .Include(n => n.Department)
                .ToListAsync();
            return _mapper.MapToDtoList(nurses);
        }

        public async Task<IEnumerable<NurseResponseDTO>> GetNurseByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Enumerable.Empty<NurseResponseDTO>();
            var nurses = await _context.Nurses
                .Include(n => n.User)
                .Include(n => n.Department)
                .Where(n => EF.Functions.Like(n.Name, $"%{name}%"))
                .ToListAsync();
            return _mapper.MapToDtoList(nurses);
        }

        public async Task<NurseResponseDTO> CreateNurseAsync(NurseCreate nurseCreateDto)
        {
            var nurse = _mapper.MapToEntity(nurseCreateDto);
            nurse.CreateDate = DateTime.Now;
            nurse.UpdateDate = DateTime.Now;

            _context.Nurses.Add(nurse);
            await _context.SaveChangesAsync();

            return _mapper.MapToDto(nurse);
        }

        public async Task<string> CheckUniqueCodeAsync()
        {
            string newCode;
            bool isExist;

            do
            {
                newCode = GenerateCode.GeneratePatientCode();
                _context.ChangeTracker.Clear();
                isExist = await _context.Nurses.AnyAsync(n => n.Code == newCode);
            }
            while (isExist);

            return newCode;
        }

        public async Task<NurseRegisterResponse> NurseRegisterAsync(NurseRegisterRequest request)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == request.DepartmentId);
            if (department == null)
            {
                throw new Exception("DepartmentId không tồn tại.");
            }
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new Exception("Email đã tồn tại");
            }
            if (await _context.Nurses.AnyAsync(u => u.Phone == request.Phone))
            {
                throw new Exception("Số điện thoại đã tồn tại");
            }
            if (await _context.Nurses.AnyAsync(u => u.CCCD == request.CCCD))
            {
                throw new Exception("CCCD đã tồn tại");
            }
            request.FullName = request.FullName.Trim();
            if (string.IsNullOrEmpty(request.FullName))
                throw new Exception("Không được để trống tên");

            if (!Regex.IsMatch(request.FullName, @"^[a-zA-ZÀ-ỹ\s]+$"))
                throw new Exception("Tên không được chứa kí tự đặc biệt");
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var newUser = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                Status = UserStatus.Active
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            var nurse = new Nurse
            {
                Code = request.Code,
                Name = request.FullName,
                Gender = request.Gender,
                Dob = request.Dob,
                CCCD = request.CCCD,
                Phone = request.Phone,
                ImageURL = "",
                Status = NurseStatus.Available,
                CreateDate = DateTime.Now.AddHours(7),
                UpdateDate = DateTime.Now.AddHours(7),
                CreateBy = request.FullName,
                UpdateBy = request.FullName,
                DepartmentId = request.DepartmentId,
                UserId = newUser.Id,
            };
            if (!string.IsNullOrEmpty(request.Code) && request.Code != "string")
            {
                nurse.Code = request.Code;
            }
            else
            {
                nurse.Code = await CheckUniqueCodeAsync();
            }

            while (await _context.Nurses.AnyAsync(n => n.Code == nurse.Code))
            {
                nurse.Code = await CheckUniqueCodeAsync();
            }

            await _context.Nurses.AddAsync(nurse);

            var nurseRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Trim().ToLower() == "nurse");
            if (nurseRole != null)
            {
                await _context.User_Roles.AddAsync(new User_Role
                {
                    UserId = newUser.Id,
                    RoleId = nurseRole.Id
                });
            }
            await _context.SaveChangesAsync();
            return new NurseRegisterResponse
            {
                NurseId = nurse.Id,
                FullName = nurse.Name,
                Email = newUser.Email,
            };
        }

        public async Task<NurseResponseDTO> UpdateNurseAsync(int id, NurseUpdate nurseUpdateDto)
        {
            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse == null) return null;

            _mapper.MapToEntity(nurseUpdateDto, nurse);
            nurse.UpdateDate = DateTime.Now;

            _context.Nurses.Update(nurse);
            await _context.SaveChangesAsync();

            return _mapper.MapToDto(nurse);
        }

        public async Task<bool> DeleteNurseAsync(int id, NurseDelete nurseDeleteDto)
        {
            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse == null) return false;

            _context.Nurses.Remove(nurse);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}