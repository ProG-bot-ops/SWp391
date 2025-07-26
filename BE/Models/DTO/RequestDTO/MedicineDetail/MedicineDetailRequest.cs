namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO
{
    public class MedicineDetailRequest
    {
        public int MedicineId { get; set; }
        public string Ingredients { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? Manufacturer { get; set; }
        public string Warning { get; set; }
        public string StorageInstructions { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
    }

}
