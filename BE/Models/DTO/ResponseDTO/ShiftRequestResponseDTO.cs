using System;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO.ShiftRequest
{
    public class ShiftRequestResponseDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int ShiftId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? DoctorName { get; set; }
        public string? ShiftType { get; set; }
        public DateTime? ShiftDate { get; set; }
    }
}