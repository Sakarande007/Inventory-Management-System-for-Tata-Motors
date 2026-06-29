namespace PaintShopIMS.Models
{
    /// <summary>
    /// Represents a master part record in the inventory.
    /// </summary>
    public class PartMaster
    {
        public int PartId { get; set; }
        public string PartNo { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
    }
}
