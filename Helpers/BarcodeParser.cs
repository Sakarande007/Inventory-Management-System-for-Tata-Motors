using System;
using PaintShopIMS.Models;

namespace PaintShopIMS.Helpers
{
    public static class BarcodeParser
    {
        /// <summary>
        /// Parses a raw barcode string into a ParsedBarcode object.
        /// Expected layout: PartNo|YYYY-MM-DD|SerialNumber
        /// </summary>
        public static ParsedBarcode? Parse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            string originalRaw = raw;
            string? defectCode = null;

            // Check if it's a rework QR
            if (raw.Contains("|RW:"))
            {
                var partsWithRework = raw.Split(new[] { "|RW:" }, StringSplitOptions.None);
                if (partsWithRework.Length == 2)
                {
                    raw = partsWithRework[0]; // The original part of the QR
                    defectCode = partsWithRework[1];
                }
            }

            var parts = raw.Split('|');
            if (parts.Length == 3)
            {
                if (DateTime.TryParse(parts[1], out DateTime scanDate))
                {
                    return new ParsedBarcode
                    {
                        PartNo = parts[0].Trim(),
                        Date = scanDate,
                        SerialNumber = parts[2].Trim(),
                        IsValid = true,
                        DefectCode = defectCode,
                        OriginalRawData = originalRaw
                    };
                }
            }

            // Fallback for compact format (e.g., F120310326032627220)
            // Layout: PartNo (8 chars) + Date YYMMDD (6 chars) + Serial (remaining chars)
            if (raw.Length >= 19 && !raw.Contains("|"))
            {
                string partNo = raw.Substring(0, 8);
                string dateStr = raw.Substring(8, 6);
                string serial = raw.Substring(14);

                if (DateTime.TryParseExact(dateStr, "yyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime scanDateCompact))
                {
                    return new ParsedBarcode
                    {
                        PartNo = partNo,
                        Date = scanDateCompact,
                        SerialNumber = serial,
                        IsValid = true,
                        DefectCode = defectCode,
                        OriginalRawData = originalRaw
                    };
                }
            }

            return null;
        }
    }
}
