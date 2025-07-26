namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicalRecord
{
    public class MedicalRecordUpdateRequest
    {
        public string Diagnosis { get; set; }
        public string TestResults { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int? DiseaseId { get; set; }
        public int AppointmentId { get; set; }
        public int? PrescriptionId { get; set; }

        public MedicalRecordUpdateRequest()
        {
        }

        public MedicalRecordUpdateRequest(
            string diagnosis,
            string testResults,
            string? notes,
            string status,
            int doctorId,
            int patientId,
            int? diseaseId,
            int appointmentId,
            int? prescriptionId)
        {
            Diagnosis = diagnosis;
            TestResults = testResults;
            Notes = notes;
            Status = status;
            DoctorId = doctorId;
            PatientId = patientId;
            DiseaseId = diseaseId;
            AppointmentId = appointmentId;
            PrescriptionId = prescriptionId;
        }
    }
}