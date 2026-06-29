using System;
using System.IO;

namespace PaintShopIMS.Helpers
{
    /// <summary>
    /// Reads a label template file and replaces placeholders with real print data.
    /// Changing label.txt is the ONLY change needed to update the print format.
    /// </summary>
    public static class LabelTemplateProcessor
    {
        /// <summary>
        /// Processes the template and returns print-ready content with all placeholders resolved.
        /// </summary>
        /// <param name="templatePath">Full path to label.txt</param>
        /// <param name="partNo">Part number  → replaces @PartNo / @Partno</param>
        /// <param name="serial">Serial number → replaces @Serial</param>
        /// <param name="rewCode">Defect code   → replaces @Rewcode / @rewcode</param>
        /// <param name="originalQr">Original scanned QR string. Combined with rewCode to build @QRDATA.</param>
        /// <returns>Fully resolved string, ready to write to print.txt</returns>
        public static string Process(
            string templatePath,
            string partNo,
            string serial,
            string rewCode,
            string originalQr)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Label template not found at: {templatePath}");

            string template = File.ReadAllText(templatePath);

            var now = DateTime.Now;

            // Computed date fields
            string yy    = now.ToString("yy");            // e.g. "26"
            string jDate = now.DayOfYear.ToString("D3");  // Julian date, e.g. "118"

            // Guard against nulls
            partNo     ??= string.Empty;
            serial     ??= string.Empty;
            rewCode    ??= string.Empty;
            originalQr ??= string.Empty;

            // Build the final QR payload:  OriginalQR|RW:DefectCode
            // e.g.  F120310326032627222|RW:01
            string finalQr = $"{originalQr}|RW:{rewCode}";

            // Replace every known placeholder (case-sensitive variants included)
            return template
                .Replace("@QRDATA",  finalQr)    // QR barcode data — MUST come first
                .Replace("@PartNo",  partNo)
                .Replace("@Partno",  partNo)
                .Replace("@Serial",  serial)
                .Replace("@Rewcode", rewCode)
                .Replace("@rewcode", rewCode)
                .Replace("@YY",      yy)
                .Replace("@Jdate",   jDate);
        }
    }
}
