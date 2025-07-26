using System;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.PatientFilter
{
    public class PatientScheduleRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DoctorId phải lớn hơn 0")]
        public int DoctorId { get; set; }

        public DateTime? Date { get; set; }

        public string? Status { get; set; }

        public string? PatientName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber phải lớn hơn 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize phải từ 1 đến 100")]
        public int PageSize { get; set; } = 10;

        public override string ToString()
        {
            return $"DoctorId: {DoctorId}, Date: {Date}, Status: {Status}, PatientName: {PatientName}, Page: {PageNumber}, Size: {PageSize}";
        }
    }
}

