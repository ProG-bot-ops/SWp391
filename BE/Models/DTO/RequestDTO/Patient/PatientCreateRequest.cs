using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SWP391_SE1914_ManageHospital.Ultility;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient
{
    public class PatientCreateRequest
    {
        [Required]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^\d{12}$")]
        public string CCCD { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        public int Gender { get; set; }

        [Required]
        [RegularExpression(@"^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$")]
        public string EmergencyContact { get; set; }
    }
}