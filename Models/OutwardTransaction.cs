using System;

namespace PaintShopIMS.Models
{
    /// <summary>
    /// Represents a scanned outward transaction linking back to an Inward scan.
    /// </summary>
    public class OutwardTransaction
    {
        public int OutwardId { get; set; }
        public string PartNo { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string ScanDate { get; set; } = string.Empty;
        public string ScannedBy { get; set; } = string.Empty;
        public string? DefectCode { get; set; }
    }
}
