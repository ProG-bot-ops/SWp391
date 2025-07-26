namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineInventoryPageDTO
    {
        public IEnumerable<MedicineInventoryResponseDTO> Items { get; set; }
        public int TotalPages { get; set; }

        public MedicineInventoryPageDTO() { 
            Items = new List<MedicineInventoryResponseDTO>();
        }

        public MedicineInventoryPageDTO(List<MedicineInventoryResponseDTO> items, int totalPages)
        {
            Items = items;
            TotalPages = totalPages;
        }
    }
}
