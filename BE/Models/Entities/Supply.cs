using static SWP391_SE1914_ManageHospital.Ultility.Status;

namespace SWP391_SE1914_ManageHospital.Models.Entities;

public class Supply : BaseEntity
{
    public SupplyStatus Status { get; set; }
    public string Description { get; set; }
    
    public int AppointmentId { get; set; }
    public virtual Appointment Appointment { get; set; }
    public virtual ICollection<Supply_Inventory> Supply_Inventories { get; set; } = new List<Supply_Inventory>();

}
