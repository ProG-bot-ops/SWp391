using System;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class DiseaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DiseaseStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }

        public DiseaseResponse()
        {
        }

        public DiseaseResponse(
            int id,
            string name,
            string code,
            string description,
            DiseaseStatus status,
            DateTime createDate,
            DateTime? updateDate,
            string createBy,
            string updateBy)
        {
            Id = id;
            Name = name;
            Code = code;
            Description = description;
            Status = status;
            CreateDate = createDate;
            UpdateDate = updateDate;
            CreateBy = createBy;
            UpdateBy = updateBy;
        }
    }
}