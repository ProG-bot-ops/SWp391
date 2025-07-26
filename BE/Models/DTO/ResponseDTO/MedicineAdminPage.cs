namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineAdminPage
    {
        public IEnumerable<MedicineAdmin> Items { get; set; }
        public int TotalPages { get; set; }

        public MedicineAdminPage() 
        {
            Items = new List<MedicineAdmin>();
        }

        public MedicineAdminPage(List<MedicineAdmin> items, int totalPages)
        {
            Items = items;
            TotalPages = totalPages;
        }
    }
}
