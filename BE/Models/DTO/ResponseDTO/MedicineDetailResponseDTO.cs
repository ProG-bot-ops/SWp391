using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineDetailResponseDTO
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string Ingredients { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? Manufacturer { get; set; }
        public string Warning { get; set; }
        public string StorageInstructions { get; set; }
        public MedicineStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public string Description { get; set; }
    }

}
