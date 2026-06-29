namespace PaintShopIMS.Models
{
    /// <summary>
    /// Represents a customer master record.
    /// </summary>
    public class CustomerMaster
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
    }
}
