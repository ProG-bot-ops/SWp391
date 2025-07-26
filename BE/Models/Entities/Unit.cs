namespace SWP391_SE1914_ManageHospital.Models.Entities
{
    public class Unit
    {
        public int Id { get; set; }             
        public string Name { get; set; }        
        public int Status { get; set; }
        public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
        public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();

    }
}
