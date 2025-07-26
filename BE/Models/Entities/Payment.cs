namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Payment : BaseEntity
{
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Payer { get; set; }
    public string? Notes { get; set; }
    public virtual ICollection<Payment_Invoice> Payment_Invoices { get; set; } = new List<Payment_Invoice>();
}
