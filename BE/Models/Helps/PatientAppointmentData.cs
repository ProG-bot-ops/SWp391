using System;
using SWP391_SE1914_ManageHospital.Models.Entities;

namespace SWP391_SE1914_ManageHospital.Models.Helps
{
    /// hỗ trợ truyền dữ liệu giữa service và mapper
    public class PatientAppointmentData
    {
        public Patient Patient { get; set; }
        public DateTime ExaminationTime { get; set; }
        public string AppointmentStatus { get; set; }
        public int AppointmentId { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PatientId { get; set; }
        public int ClinicId { get; set; }
        public int ReceptionId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public bool IsSend { get; set; }

        public PatientAppointmentData() 
        {
            this.Patient = null;
            this.ExaminationTime = DateTime.MinValue;
            this.AppointmentStatus = string.Empty;
            this.AppointmentId = 0;
            this.Note = string.Empty;
            this.Code = string.Empty;
            this.StartTime = DateTime.MinValue;
            this.EndTime = DateTime.MinValue;
            this.PatientId = 0;
            this.ClinicId = 0;
            this.ReceptionId = 0;
            this.Name = string.Empty;
            this.CreateDate = DateTime.MinValue;
            this.UpdateDate = DateTime.MinValue;
            this.CreateBy = string.Empty;
            this.UpdateBy = string.Empty;
            this.IsSend = false;
        }
    }
}