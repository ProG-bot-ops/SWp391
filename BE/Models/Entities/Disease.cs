using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Disease : BaseEntity
{
    public string Description { get; set; }
    public DiseaseStatus Status { get; set; }
    public virtual ICollection<Medical_Record> Medical_Records { get; set; } = new List<Medical_Record>();
    public virtual ICollection<DiseaseDetail> DiseaseDetails { get; set; } = new List<DiseaseDetail>();
}
