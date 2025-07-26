using System.ComponentModel.DataAnnotations;
using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.DTO.RequestDTO.Medicine
{
    public class MedicineRequest
    {
        public string? ImageUrl { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public int Prescribed { get; set; }
        public int UnitId { get; set; }

        public decimal UnitPrice { get; set; }
        public int MedicineCategoryId { get; set; }

        
    }

}
