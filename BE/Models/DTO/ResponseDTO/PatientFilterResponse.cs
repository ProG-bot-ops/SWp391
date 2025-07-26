using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class PatientFilterResponse
    {
        /// ID của bệnh nhân
        public int Id { get; set; }

        /// Tên của bệnh nhân
        public string Name { get; set; }

        /// Thời gian khám bệnh
        public DateTime ExaminationTime { get; set; }

        /// Trạng thái cuộc hẹn
        public string AppointmentStatus { get; set; }

        public PatientFilterResponse()
        {
            this.Id = 0;
            this.Name = string.Empty;
            this.ExaminationTime = DateTime.MinValue;
            this.AppointmentStatus = string.Empty;
        }
    }
}