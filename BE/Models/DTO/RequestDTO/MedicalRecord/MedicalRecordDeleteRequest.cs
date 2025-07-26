namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.MedicalRecord
{
    public class MedicalRecordDeleteRequest
    {
        public int Id { get; set; }

        public MedicalRecordDeleteRequest()
        {
        }

        public MedicalRecordDeleteRequest(int id)
        {
            Id = id;
        }
    }
}