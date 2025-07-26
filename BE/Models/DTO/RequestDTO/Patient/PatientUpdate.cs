using static SWP391_SE1914_ManageHospital.Ultility.Status;
using System.ComponentModel.DataAnnotations;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Patient
{
    public class PatientUpdate
    {
        
        public string Name { get; set; }
        public string Code { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public string CCCD { get; set; }
        public string Phone { get; set; }
        public string? EmergencyContact { get; set; }
        public string Address { get; set; }
        public string? InsuranceNumber { get; set; }
        public string? Allergies { get; set; }

        [EnumDataType(typeof(PatientStatus))]
        public PatientStatus Status { get; set; }
        public string? BloodType { get; set; }
        public string? ImageURL { get; set; }
        public int UserId { get; set; }
        
        public DateTime? UpdateDate { get; set; }
       
        public string? UpdateBy { get; set; }

        public PatientUpdate()
        {
            
        }

        public PatientUpdate( string name, string code, Gender gender, DateTime dob, string cCCD, string phone, string emergencyContact, string address, string insuranceNumber, string? allergies, PatientStatus status, string bloodType, string? imageURL, int userId,  DateTime? updateDate, string? updateBy)
        {
            
            Name = name;
            Code = code;
            Gender = gender;
            Dob = dob;
            CCCD = cCCD;
            Phone = phone;
            EmergencyContact = emergencyContact;
            Address = address;
            InsuranceNumber = insuranceNumber;
            Allergies = allergies;
            Status = status;
            BloodType = bloodType;
            ImageURL = imageURL;
            UserId = userId;
           
            UpdateDate = updateDate;
            
            UpdateBy = updateBy;
        }
    }
}
