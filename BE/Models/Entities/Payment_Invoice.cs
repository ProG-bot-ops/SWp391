namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Payment_Invoice
{
    public int Id { get; set; }
    public decimal AmountPaid { get; set; }
    public int PaymentId { get; set; }
    public int InvoiceId { get; set; }
    public virtual Invoice Invoice { get; set; } = null!;
    public virtual Payment Payment { get; set; } = null!;

}
