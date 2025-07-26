namespace SWP391_SE1914_ManageHospital.Ultility;

public static class Status
{
    public enum AppointmentStatus
    {
        Scheduled = 0,      // Chưa tới
        InProgress = 1,     // Đang khám
        Late = 2,           // Đến muộn
        Completed = 3,      // Hoàn thành
        Cancelled = 4       // Đã hủy
    }
    public enum Gender
    {
        Male = 0, Female = 1
    }
    public enum ClinicStatus
    {
        Available = 0, Unavailable = 1, UnderMaintenance = 2
    }
    public enum DepartmentStatus
    {
        Active = 0, Inactive = 1
    }
    public enum DiseaseStatus
    {
        Active = 0, Resolved = 1, 
    }
    public enum DiseaseDetailStatus
    {
        Diagnosed = 0, Recovered = 1, Ongoing = 2
    }
    public enum DoctorStatus
    {
        Available = 0, Unavailable = 1
    }
    public enum DoctorAppointmentStatus
    {
        Assigned = 0, Completed = 1, Cancelled = 2
    }
    public enum FeedbackStatus
    {
        Visible = 0, Hidden = 1
    }
    public enum InsuranceStatus
    {
        Valid = 0, Expired = 1
    }
    public enum InvoiceStatus
    {
        Unpaid = 0, Paid = 1, PartiallyPaid = 2, Cancelled = 3
    }
    public enum InvoiceDetailStatus
    {
        Normal = 0, Discounted = 1
    }
    public enum MedicalRecordStatus
    {
        Open = 0, Closed = 1, Archived = 2
    }
    public enum MedicineStatus
    {
        Active = 0, Discontinued = 1
    }
    
    public enum MedicineInventoryStatus
    {
        InStock = 0, OutOfStock = 1, Discontinued = 2
    }
    public enum MedicineCategoryStatus
    {
        InStock = 0, OutOfStock = 1
    }
    public enum NotificationStatus
    {
        Unread = 0, Read = 1
    }
    public enum NurseStatus
    {
        Available = 0, Unavailable = 1
    }
    public enum NurseAppointmentStatus
    {
        Assigned = 0, Completed = 1
    }
    public enum PatientStatus
    {
        Active = 0, Banned = 1
    }
    public enum PaymentStatus
    {
        Pending = 0, Completed = 1, Failed = 2, Refunded = 3
    }
    public enum PrescriptionStatus
    {
        New = 0, Dispensed = 1, Cancelled = 2
    }
    public enum PrescriptionDetailStatus
    {
        New = 0, Dispensed = 1, Cancelled = 2
    }
    public enum ReceptionStatus
    {
        Active = 0, OnLeave = 1
    }
    public enum SupplyStatus
    {
        InStock = 0, OutOfStock = 1
    }
    public enum SupplyInventoryStatus
    {
        InStock = 0, OutOfStock = 1
    }
    public enum UserStatus
    {
        Active = 0, Banned = 1
    }

    public enum ServiceStatus
    {
        Active = 0, Inactive = 1
    }

    public enum ClinicType
    {
        Consultation = 0, Lab = 1
    }

    public enum PrescribedMedication
    {
        Yes = 1, No = 0 
    }
}
