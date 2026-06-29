using System;

namespace PaintShopIMS.Models
{
    /// <summary>
    /// A unified report row combining inward and outward history.
    /// </summary>
    public class TransactionReport
    {
        public string Type { get; set; } = string.Empty;
        public string PartNo { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime ScanDate { get; set; }
        public string ScannedBy { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Remark { get; set; }
        public string? DefectCode { get; set; }
    }
}
