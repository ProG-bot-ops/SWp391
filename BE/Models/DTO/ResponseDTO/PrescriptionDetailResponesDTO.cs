// Models/DTO/ResponseDTO/PrescriptionDetailResponseDTO.cs
namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class PrescriptionDetailResponseDTO
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }  // Thêm MedicineName để hiển thị tên thuốc
        public int Quantity { get; set; }
        public string Usage { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        // MỚI: giá đơn vị và tổng tiền
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

}
