using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PaintShopIMS.Helpers
{
    /// <summary>
    /// Handles template-based label printing.
    /// Flow: Read label.txt → Replace placeholders → Write print.txt (ASCII) → Run print.bat
    /// </summary>
    public static class PrintHelper
    {
        private const string TemplatePath = @"C:\PaintShopIMS\PrintTemplates\label.txt";
        private const string OutputFolder = @"C:\PaintShopIMS\PrintQueue";
        private const string PrintFile    = @"C:\PaintShopIMS\PrintQueue\print.txt";
        private const string BatFile      = @"C:\PaintShopIMS\PrintQueue\print.bat";
        private const string DebugLog     = @"C:\PaintShopIMS\PrintQueue\debug_log.txt";


        /// <summary>
        /// Prints a rework label using the dynamic template system.
        /// Changing label.txt is the ONLY thing needed to update the print format.
        /// </summary>
        /// <param name="originalQr">The FULL original scanned QR string. Used to build: OriginalQR|RW:DefectCode</param>
        public static async Task PrintLabelAsync(string partNo, string serial, string rewCode, string originalQr)
        {
            try
            {
                if (!File.Exists(TemplatePath))
                    throw new Exception("Label template missing.");

                // ── 4. Compute the final QR payload ──────────────────────────
                //    This is what goes into @QRDATA in the template
                string finalQrPayload = $"{originalQr}|RW:{rewCode}";    // correct path

                // ── 5. Write debug log (open C:\PaintShopIMS\PrintQueue\debug_log.txt to verify) ──
                string debugEntry =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{Environment.NewLine}" +
                    $"  STEP 1 - RAW INPUT{Environment.NewLine}" +
                    $"    originalQr (full scan) : [{originalQr}]{Environment.NewLine}" +
                    $"    partNo                 : [{partNo}]{Environment.NewLine}" +
                    $"    serial                 : [{serial}]{Environment.NewLine}" +
                    $"    rewCode                : [{rewCode}]{Environment.NewLine}" +
                    $"  STEP 2 - QR PAYLOAD BUILT{Environment.NewLine}" +
                    $"    finalQrPayload (@QRDATA): [{finalQrPayload}]{Environment.NewLine}" +
                    $"  ──────────────────────────────────────{Environment.NewLine}";

                await File.AppendAllTextAsync(DebugLog, debugEntry, Encoding.UTF8);
                Debug.WriteLine(debugEntry);

                // ── 6. Process template → inject real data ───────────────────
                string finalContent = LabelTemplateProcessor.Process(
                    templatePath: TemplatePath,
                    partNo:       partNo,
                    serial:       serial,
                    rewCode:      rewCode    ?? string.Empty,
                    originalQr:   originalQr ?? string.Empty
                );

                // ── 7. Write print.txt in ASCII (required by label printers) ─
                await File.WriteAllTextAsync(PrintFile, finalContent, Encoding.ASCII);

                // ── 8. Trigger the BAT to send file to printer ───────────────
                Process.Start(new ProcessStartInfo
                {
                    FileName        = BatFile,
                    UseShellExecute = true,
                    CreateNoWindow  = true
                });

                Debug.WriteLine("[PrintHelper] print.bat launched successfully.");
            }
            catch (Exception ex)
            {
                // Non-fatal: log and continue — do not crash the UI
                string errEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {ex.Message}{Environment.NewLine}";
                await File.AppendAllTextAsync(DebugLog, errEntry, Encoding.UTF8);
                Debug.WriteLine($"[PrintHelper] ERROR: {ex.Message}");
            }
        }
    }
}
