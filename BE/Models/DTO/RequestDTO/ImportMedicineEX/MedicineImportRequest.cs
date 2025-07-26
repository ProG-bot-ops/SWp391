namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.ImportMedicineEX
{
public class MedicineImportRequest
{
    public string ImportCode { get; set; } = string.Empty;
    public string ImportName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
    public string Notes { get; set; } = string.Empty;

    public List<MedicineImportDetailRequest> Details { get; set; } = new();
}
}
