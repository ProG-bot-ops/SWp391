namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class ShiftRequest
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int ShiftId { get; set; }
    public string? RequestType { get; set; }
    public string? Reason { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }

    // Navigation properties
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Doctor_Shift Shift { get; set; } = null!;
} 