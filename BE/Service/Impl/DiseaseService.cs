using Microsoft.EntityFrameworkCore;
using SWP391_SE1914_ManageHospital.Data;
using SWP391_SE1914_ManageHospital.Mapper;
using SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO;
using SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Disease;
using SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;
using SWP391_SE1914_ManageHospital.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWP391_SE1914_ManageHospital.Service.Impl
{
    public class DiseaseService : IDiseaseService
    {
        private readonly ApplicationDBContext _context;
        private readonly IDiseaseMapper _diseaseMapper;

        public DiseaseService(ApplicationDBContext context, IDiseaseMapper diseaseMapper)
        {
            _context = context;
            _diseaseMapper = diseaseMapper;
        }

        public IEnumerable<DiseaseResponse> GetAllDiseases()
        {
            var diseases = _context.Diseases.ToList();
            return diseases.Select(d => _diseaseMapper.MapToResponse(new DiseaseDTO
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                Status = d.Status,
                CreateDate = d.CreateDate,
                UpdateDate = d.UpdateDate,
                CreateBy = d.CreateBy,
                UpdateBy = d.UpdateBy
            })).ToList();
        }

        public DiseaseResponse GetDiseaseDetail(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID bệnh không hợp lệ");

            var disease = _context.Diseases.Find(id);
            if (disease == null)
                return null;

            var diseaseDTO = new DiseaseDTO
            {
                Id = disease.Id,
                Name = disease.Name,
                Code = disease.Code,
                Description = disease.Description,
                Status = disease.Status,
                CreateDate = disease.CreateDate,
                UpdateDate = disease.UpdateDate,
                CreateBy = disease.CreateBy,
                UpdateBy = disease.UpdateBy
            };

            return _diseaseMapper.MapToResponse(diseaseDTO);
        }

        public DiseaseResponse CreateDisease(DiseaseCreateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Dữ liệu tạo mới bệnh không được để trống");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Tên bệnh không được để trống");

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("Mã bệnh không được để trống");

            var diseaseEntity = new Disease
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                Status = request.Status,
                CreateDate = DateTime.Now,
                CreateBy = request.CreateBy
            };

            _context.Diseases.Add(diseaseEntity);
            _context.SaveChanges();

            var createdDiseaseDTO = new DiseaseDTO
            {
                Id = diseaseEntity.Id,
                Name = diseaseEntity.Name,
                Code = diseaseEntity.Code,
                Description = diseaseEntity.Description,
                Status = diseaseEntity.Status,
                CreateDate = diseaseEntity.CreateDate,
                CreateBy = diseaseEntity.CreateBy
            };

            return _diseaseMapper.MapToResponse(createdDiseaseDTO);
        }

        public DiseaseResponse UpdateDisease(int id, DiseaseUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Dữ liệu cập nhật bệnh không được để trống");

            if (id <= 0)
                throw new ArgumentException("ID bệnh không hợp lệ");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Tên bệnh không được để trống");

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("Mã bệnh không được để trống");

            var disease = _context.Diseases.Find(id);
            if (disease == null)
                throw new ArgumentException($"Không tìm thấy bệnh với ID: {id}");

            disease.Name = request.Name;
            disease.Code = request.Code;
            disease.Description = request.Description;
            disease.Status = request.Status;
            disease.UpdateDate = DateTime.Now;
            disease.UpdateBy = request.UpdateBy;

            _context.Diseases.Update(disease);
            _context.SaveChanges();

            var updatedDiseaseDTO = new DiseaseDTO
            {
                Id = disease.Id,
                Name = disease.Name,
                Code = disease.Code,
                Description = disease.Description,
                Status = disease.Status,
                CreateDate = disease.CreateDate,
                UpdateDate = disease.UpdateDate,
                CreateBy = disease.CreateBy,
                UpdateBy = disease.UpdateBy
            };

            return _diseaseMapper.MapToResponse(updatedDiseaseDTO);
        }

        public bool DeleteDisease(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID bệnh không hợp lệ");

            var disease = _context.Diseases.Find(id);
            if (disease == null)
                return false;

            // Kiểm tra xem disease có liên quan đến medical record nào không
            if (_context.Medical_Records.Any(mr => mr.DiseaseId == id))
                return false;

            _context.Diseases.Remove(disease);
            _context.SaveChanges();
            return true;
        }
    }
}