namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Disease
{
    public class DiseaseDeleteRequest
    {
        public int Id { get; set; }

        public DiseaseDeleteRequest()
        {
        }

        public DiseaseDeleteRequest(int id)
        {
            Id = id;
        }
    }
}