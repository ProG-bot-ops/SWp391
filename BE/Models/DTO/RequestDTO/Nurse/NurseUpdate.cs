using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse
{
    public class NurseUpdate
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public string CCCD { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? ImageURL { get; set; }
        public NurseStatus Status { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string UpdateBy { get; set; } = string.Empty;
    }
}