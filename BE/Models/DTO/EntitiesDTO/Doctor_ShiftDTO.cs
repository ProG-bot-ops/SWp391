using System;
using static SWP391_SE1914_ManageHospital.Ultility.Status;
using SWP391_SE1914_ManageHospital.Models.Entities;
namespace SWP391_SE1914_ManageHospital.Models.DTO.EntitiesDTO
{
    public class Doctor_ShiftDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime ShiftDate { get; set; }
        public string ShiftType { get; set; } = null!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public string? DoctorName { get; set; }
    }
}
