namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Nurse
{
    public class NurseDelete
    {
        public int Id { get; set; }
        public string DeleteBy { get; set; } = string.Empty;
        public string? Reason { get; set; } 
    }
}