using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SWP391_SE1914_ManageHospital.Ultility;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient
{
    public class PatientUpdateRequest
    {
        [Required(ErrorMessage = "ID bệnh nhân không được để trống")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã bệnh nhân không được để trống")]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Tên bệnh nhân không được để trống")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CCCD không được để trống")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD phải có đúng 12 chữ số")]
        [JsonPropertyName("cccd")]
        public string CCCD { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [JsonPropertyName("dob")]
        public DateTime Dob { get; set; }

        [Range(-1, 1, ErrorMessage = "Giới tính phải là 0 (Nam), 1 (Nữ) hoặc -1 (Giữ nguyên)")]
        [JsonPropertyName("gender")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("bloodType")]
        public string? BloodType { get; set; }

        [JsonPropertyName("insuranceNumber")]
        public string? InsuranceNumber { get; set; }

        [JsonPropertyName("allergies")]
        public string? Allergies { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Active";
    }
} 