using System;

namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicalRecordDetailResponse
    {
        public int Id { get; set; }

        public string Diagnosis { get; set; }

        public string TestResults { get; set; }

        public string? Notes { get; set; }

        public string Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string DoctorName { get; set; }

        public string PatientName { get; set; }

        public string DiseaseName { get; set; }

        public int AppointmentId { get; set; }

        public int? PrescriptionId { get; set; }

        public MedicalRecordDetailResponse()
        {
            this.Id = 0;
            this.Diagnosis = string.Empty;
            this.TestResults = string.Empty;
            this.Notes = null;
            this.Status = string.Empty;
            this.CreateDate = DateTime.MinValue;
            this.DoctorName = string.Empty;
            this.PatientName = string.Empty;
            this.DiseaseName = string.Empty;
            this.AppointmentId = 0;
            this.PrescriptionId = null;
        }

        public MedicalRecordDetailResponse(
            int id,
            string diagnosis,
            string testResults,
            string? notes,
            string status,
            DateTime createDate,
            string doctorName,
            string patientName,
            string diseaseName,
            int appointmentId,
            int? prescriptionId)
        {
            this.Id = id;
            this.Diagnosis = diagnosis;
            this.TestResults = testResults;
            this.Notes = notes;
            this.Status = status;
            this.CreateDate = createDate;
            this.DoctorName = doctorName;
            this.PatientName = patientName;
            this.DiseaseName = diseaseName;
            this.AppointmentId = appointmentId;
            this.PrescriptionId = prescriptionId;
        }
    }
}