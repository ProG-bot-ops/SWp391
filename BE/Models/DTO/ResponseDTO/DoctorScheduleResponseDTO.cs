using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest
{
    public class DoctorScheduleResponseDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime ShiftDate { get; set; }
        public string ShiftType { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public string? DeleteBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}