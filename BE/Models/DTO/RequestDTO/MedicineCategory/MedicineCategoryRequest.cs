namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO
{
    public class MedicineCategoryRequest
    {
        public string? ImageUrl { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
