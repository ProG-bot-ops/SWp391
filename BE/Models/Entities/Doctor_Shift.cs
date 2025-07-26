namespace SWP391_SE1914_ManageHospital.Models.Entities
{
    public class Doctor_Shift
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        public DateTime ShiftDate { get; set; }
        public string ShiftType { get; set; } = null!;  // "Morning" or "Afternoon"

        public TimeSpan StartTime { get; set; }              
        public TimeSpan EndTime { get; set; }               
        public string? Notes { get; set; }
        
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
    }
}
