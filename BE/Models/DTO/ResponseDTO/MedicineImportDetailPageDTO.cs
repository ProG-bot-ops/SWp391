namespace SWP391_SE1914_ManageHospital.Models.DTO.ResponseDTO
{
    public class MedicineImportDetailPageDTO
    {
        public List<MedicineImportDetailResponseDTO> Items { get; set; }
        public int TotalPages { get; set; }

        public MedicineImportDetailPageDTO()
        {
            Items = new List<MedicineImportDetailResponseDTO>();
        }

        public MedicineImportDetailPageDTO(List<MedicineImportDetailResponseDTO> items, int totalPages)
        {
            Items = items;
            TotalPages = totalPages;
        }
    }
}
