using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient
{
    public class PatientAndAppointmentCreateRequest
    {
        // Patient Information
        [Required]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [Required]
        [JsonPropertyName("phone")]
        [RegularExpression(@"^0(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$")]
        public string Phone { get; set; }

        [Required]
        [JsonPropertyName("cccd")]
        [RegularExpression(@"^\d{12}$")]
        public string CCCD { get; set; }

        [Required]
        [JsonPropertyName("email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [Required]
        [JsonPropertyName("dob")]
        public DateTime Dob { get; set; }

        [Required]
        [JsonPropertyName("gender")]
        public int Gender { get; set; }

        [Required]
        [JsonPropertyName("emergencyContact")]
        [RegularExpression(@"^0(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$")]
        public string EmergencyContact { get; set; }

        // Appointment Information
        [Required]
        [JsonPropertyName("clinicId")]
        public int ClinicId { get; set; }

        [Required]
        [JsonPropertyName("doctorId")]
        public int DoctorId { get; set; }

        [Required]
        [JsonPropertyName("serviceId")]
        public int ServiceId { get; set; }

        [Required]
        [JsonPropertyName("appointmentDate")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [JsonPropertyName("shift")]
        public string Shift { get; set; } // "morning" hoặc "afternoon"

        [JsonPropertyName("note")]
        public string? Note { get; set; }
    }
}