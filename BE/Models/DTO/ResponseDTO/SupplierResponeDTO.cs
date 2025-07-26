namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{

    public class SupplierResponeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateBy { get; set; }
        public string? UpdateBy { get; set; }

        public SupplierResponeDTO() { }

        public SupplierResponeDTO(int id, string name, string phone, string email, string address, string code, DateTime createDate, DateTime? updateDate, string createBy, string? updateBy)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            Code = code;
            CreateDate = createDate;
            UpdateDate = updateDate;
            CreateBy = createBy;
            UpdateBy = updateBy;
        }
    }
}
