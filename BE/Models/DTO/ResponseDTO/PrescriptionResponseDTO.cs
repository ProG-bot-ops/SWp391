using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class PrescriptionResponseDTO
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public PrescriptionStatus Status { get; set; }
        public string PatientName { get; set; }  // Tên bệnh nhân
        public string PatientCCCD { get; set; }  // CCCD của bệnh nhân
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }  // Tên bác sĩ
        public string Name { get; set; }  // Tên đơn thuốc
        public string Code { get; set; }  // Mã đơn thuốc
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
    }


}
