using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineCategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Code { get; set; } = null!;
        public MedicineCategoryStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateBy { get; set; } = null!;
        public string? UpdateBy { get; set; }
    }
}
