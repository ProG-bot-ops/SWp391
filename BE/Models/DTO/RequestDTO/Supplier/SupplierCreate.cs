namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Supplier
{
    public class SupplierCreate
    {
        public string Name { get; set; }

        public string Code { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
       

        public SupplierCreate() { }

        public SupplierCreate(string name, string phone, string email, string address, string code)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            Code = code;
        }
    }
}
