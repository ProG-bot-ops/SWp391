namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class UnitResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }

        public UnitResponseDTO()
        {
            
        }

        public UnitResponseDTO(int id, string name, int status)
        {
            Id = id;
            Name = name;
            Status = status;
        }
    }
}
