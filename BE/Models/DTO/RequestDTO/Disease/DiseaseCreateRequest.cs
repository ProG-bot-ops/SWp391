using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Disease
{
    public class DiseaseCreateRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DiseaseStatus Status { get; set; }
        public string CreateBy { get; set; }

        public DiseaseCreateRequest()
        {
        }

        public DiseaseCreateRequest(
            string name,
            string code,
            string description,
            DiseaseStatus status,
            string createBy)
        {
            Name = name;
            Code = code;
            Description = description;
            Status = status;
            CreateBy = createBy;
        }
    }
}