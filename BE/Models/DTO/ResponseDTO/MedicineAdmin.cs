using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineAdmin
    {
        public int Id { get; set; }
        public string MedicineImage { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public MedicineStatus MedicineStatus { get; set; }
        public string MDescription { get; set; }
        public string MDDescription { get; set; }
        public PrescribedMedication Prescribled { get; set; }
        public string UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CategoryName { get; set; }
        public string Ingredients { get; set; }
        public string Warning { get; set; }
        public string StorageIntructions { get; set; }
    }
}

