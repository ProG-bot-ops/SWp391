using System;
namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ShiftRequest
{
    public class DoctorShiftFilterRequestDTO
    {
        public int DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
