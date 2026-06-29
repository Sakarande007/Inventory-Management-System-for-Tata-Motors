using System;

namespace PaintShopIMS.Models
{
    /// <summary>
    /// Represents a scanned inward transaction of a part.
    /// </summary>
    public class InwardTransaction
    {
        public int InwardId { get; set; }
        public string PartNo { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string ScanDate { get; set; } = string.Empty;
        public string ScannedBy { get; set; } = string.Empty;
        
        public bool IsRework { get; set; }
        public string? DefectCode { get; set; }
        public string? ModifiedQRCode { get; set; }
    }
}
