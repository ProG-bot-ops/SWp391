namespace Models.DTO.RequestDTO.Payment
{
    public class PaymentCreate
    {
        public int PatientId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string? Note { get; set; }
    }
}
