using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System.Collections.Generic;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Mapper
{
    public class NurseMapper : INurseMapper
    {
        public NurseResponseDTO MapToDto(Nurse nurse)
        {
            if (nurse == null) return null;
            return new NurseResponseDTO
            {
                Id = nurse.Id,
                Name = nurse.Name,
                Code = nurse.Code,
                Gender = nurse.Gender,
                Dob = nurse.Dob,
                CCCD = nurse.CCCD,
                Phone = nurse.Phone,
                ImageURL = nurse.ImageURL,
                Status = nurse.Status,
                UserId = nurse.UserId,
                DepartmentId = nurse.DepartmentId,
                CreateDate = nurse.CreateDate,
                UpdateDate = nurse.UpdateDate,
                CreateBy = nurse.CreateBy,
                UpdateBy = nurse.UpdateBy
            };
        }

        public Nurse MapToEntity(NurseCreate nurseCreateDto)
        {
            if (nurseCreateDto == null) return null;
            return new Nurse
            {
                Name = nurseCreateDto.Name,
                Code = nurseCreateDto.Code,
                Gender = nurseCreateDto.Gender,
                Dob = nurseCreateDto.Dob,
                CCCD = nurseCreateDto.CCCD,
                Phone = nurseCreateDto.Phone,
                ImageURL = nurseCreateDto.ImageURL,
                Status = nurseCreateDto.Status,
                UserId = nurseCreateDto.UserId,
                DepartmentId = nurseCreateDto.DepartmentId,
                CreateBy = nurseCreateDto.CreateBy
            };
        }

        public void MapToEntity(NurseUpdate nurseUpdateDto, Nurse nurse)
        {
            if (nurseUpdateDto == null || nurse == null) return;
            nurse.Name = nurseUpdateDto.Name;
            nurse.Code = nurseUpdateDto.Code;
            nurse.Gender = nurseUpdateDto.Gender;
            nurse.Dob = nurseUpdateDto.Dob;
            nurse.CCCD = nurseUpdateDto.CCCD;
            nurse.Phone = nurseUpdateDto.Phone;
            nurse.ImageURL = nurseUpdateDto.ImageURL;
            nurse.Status = nurseUpdateDto.Status;
            nurse.UserId = nurseUpdateDto.UserId;
            nurse.DepartmentId = nurseUpdateDto.DepartmentId;
            nurse.UpdateBy = nurseUpdateDto.UpdateBy;
        }

        public IEnumerable<NurseResponseDTO> MapToDtoList(IEnumerable<Nurse> nurses)
        {
            return nurses?.Select(n => MapToDto(n)).ToList() ?? new List<NurseResponseDTO>();
        }
    }
}