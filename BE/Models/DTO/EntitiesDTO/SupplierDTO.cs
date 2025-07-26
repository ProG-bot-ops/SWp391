namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string? UpdateBy { get; set; }

    }
}
