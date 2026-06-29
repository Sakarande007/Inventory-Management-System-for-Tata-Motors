using System;

namespace PaintShopIMS.Models
{
    /// <summary>
    /// Structure representing a successfully parsed barcode.
    /// </summary>
    public class ParsedBarcode
    {
        public string PartNo { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string? DefectCode { get; set; }
        public string OriginalRawData { get; set; } = string.Empty;
    }
}
