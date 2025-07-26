namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Unit
{
    public class UnitCreate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }

        public UnitCreate(int id, string name, int status)
        {
            Id = id;
            Name = name;
            Status = status;
        }
    }
}
