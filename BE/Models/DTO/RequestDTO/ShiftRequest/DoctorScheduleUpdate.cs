using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.DoctorShift
{
    public class DoctorScheduleUpdate
    {
        public int Id { get; set; }
        public DateTime ShiftDate { get; set; }
        public string ShiftType { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public string? UpdateBy { get; set; }
    }
}