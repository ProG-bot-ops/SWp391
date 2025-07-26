using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class PatientInfoAdmin
    {
        public string PatientCode { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Phone { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorName { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public PatientStatus StatusAcount { get; set; }

    }
}
