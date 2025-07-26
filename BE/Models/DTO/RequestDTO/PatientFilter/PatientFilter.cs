namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PatientFilter
{
    public class PatientFilter
    {
        /// ID của bác sĩ cần lọc danh sách bệnh nhân
        public int DoctorId { get; set; }

        /// Ngày bắt đầu lọc
        public DateTime? FromDate { get; set; }

        /// Ngày kết thúc lọc
        public DateTime? ToDate { get; set; }

        /// Tên bệnh nhân để lọc
        public string? PatientName { get; set; }

        /// Trạng thái cuộc hẹn để lọc (nếu cần)
        public string? AppointmentStatus { get; set; }
    }
}