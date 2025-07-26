namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO;

public class MedicalRecordResponse
{
    // Chỉ lấy các trường cần thiết cho danh sách
    public int Id { get; set; }
    public string Diagnosis { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime CreateDate { get; set; }
    public string DoctorName { get; set; } = default!;
    public string PatientName { get; set; } = default!;
    public string DiseaseName { get; set; } = default!;
}