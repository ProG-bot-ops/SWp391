using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Prescription
{
    public class PrescriptionRequest
    {
        public string Note { get; set; }  // Ghi chú về đơn thuốc
        public PrescriptionStatus Status { get; set; }  // Trạng thái đơn thuốc (New, Dispensed, Cancelled)
        public string PatientName { get; set; }  // Tên bệnh nhân (Thay thế PatientId)
        public string PatientCCCD { get; set; }  // CCCD của bệnh nhân (Thay thế PatientId)
        public int UserId { get; set; }  // Lấy UserId từ JWT token để xác định bác sĩ đang tạo đơn
        public string Name { get; set; }  // Tên đơn thuốc
        public string Code { get; set; }  // Mã đơn thuốc
        
    }


}
