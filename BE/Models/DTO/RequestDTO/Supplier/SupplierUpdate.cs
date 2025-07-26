namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Supplier
{
    public class SupplierUpdate
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }

        public SupplierUpdate() { }

        public SupplierUpdate(string name, string phone, string email, string address, string code, DateTime? updateDate, string? updateBy)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            Code = code;
            UpdateDate = updateDate;
            UpdateBy = updateBy;
        }
    }
}
