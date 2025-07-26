using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class DiseaseDetail : BaseEntity
{
    public string Description { get; set; }
    public DiseaseDetailStatus Status { get; set; }
    public int DiseaseId { get; set; }
    public virtual Disease Disease { get; set; }
}
