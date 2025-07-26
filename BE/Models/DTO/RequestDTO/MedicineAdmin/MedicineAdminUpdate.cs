using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicineAdmin
{
    public class MedicineAdminUpdate
    {
        // Thuộc bảng medicines
        //public string MedicineImage { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public MedicineStatus Status { get; set; }
        public decimal UnitPrice { get; set; }
        public string MDescription { get; set; }
        public PrescribedMedication Prescribed { get; set; }

        // Thay vì Name, dùng Id
        public int UnitId { get; set; }
        public int MedicineCategoryId { get; set; }

        // Thuộc bảng medicine_detail
        public string MDDescription { get; set; }
        public string Ingredients { get; set; }
        public string Warning { get; set; }
        public string StorageInstructions { get; set; }
    }
}
