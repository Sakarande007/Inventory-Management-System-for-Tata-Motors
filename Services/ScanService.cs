using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PaintShopIMS.Helpers;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;

namespace PaintShopIMS.Services
{
    public class ScanService
    {
        private readonly PartRepository _partRepo;
        private readonly InwardRepository _inwardRepo;
        private readonly OutwardRepository _outwardRepo;

        public ScanService(PartRepository partRepo, InwardRepository inwardRepo, OutwardRepository outwardRepo)
        {
            _partRepo = partRepo;
            _inwardRepo = inwardRepo;
            _outwardRepo = outwardRepo;
        }

        public async Task<(bool ok, string message)> SaveInwardAsync(string originalQr, string? defectCode, bool isRework, string? modifiedQr, string scannedBy)
        {
            var parsed = BarcodeParser.Parse(originalQr);
            if (parsed == null)
                return (false, "Invalid barcode format. Expected PartNo|Date|Serial");

            if (isRework && string.IsNullOrWhiteSpace(defectCode))
                return (false, "Defect code is required for rework parts.");

            bool partExists = await _partRepo.ExistsPartNoAsync(parsed.PartNo);
            if (!partExists)
            {
                var newPart = new PartMaster
                {
                    PartNo = parsed.PartNo,
                    PartName = "Auto-Registered Part",
                    Description = "Added automatically during inward scan",
                    Remark = ""
                };
                await _partRepo.InsertAsync(newPart);
            }

            bool serialExists = await _inwardRepo.ExistsSerialAsync(parsed.SerialNumber);
            if (serialExists)
                return (false, $"Serial {parsed.SerialNumber} has already been inwarded.");

            var transaction = new InwardTransaction
            {
                PartNo = parsed.PartNo,
                SerialNumber = parsed.SerialNumber,
                ScanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ScannedBy = scannedBy,
                IsRework = isRework,
                DefectCode = defectCode,
                ModifiedQRCode = modifiedQr
            };

            await _inwardRepo.InsertAsync(transaction);
            
            string successMsg = isRework 
                ? $"✓ Rework recorded — {parsed.PartNo} | {parsed.SerialNumber}" 
                : $"✓ Inward recorded — {parsed.PartNo} | {parsed.SerialNumber}";

            // Trigger print if it's a rework and we generated a new QR
            if (isRework && !string.IsNullOrEmpty(modifiedQr))
            {
                // ── Diagnostic: verify the full QR string at the point of printing ──
                Debug.WriteLine("[ScanService] ── PRINT TRIGGER ──");
                Debug.WriteLine($"[ScanService]   originalQr (full scan)  : [{originalQr}]");
                Debug.WriteLine($"[ScanService]   parsed.PartNo            : [{parsed.PartNo}]");
                Debug.WriteLine($"[ScanService]   parsed.SerialNumber      : [{parsed.SerialNumber}]");
                Debug.WriteLine($"[ScanService]   defectCode               : [{defectCode}]");
                Debug.WriteLine($"[ScanService]   → finalQR will be        : [{originalQr}|RW:{defectCode}]");

                await PrintHelper.PrintLabelAsync(parsed.PartNo, parsed.SerialNumber, defectCode ?? "", originalQr);
            }

            return (true, successMsg);
        }

        public async Task<(bool ok, string message)> ProcessOutward(string rawBarcode, string scannedBy)
        {
            var parsed = BarcodeParser.Parse(rawBarcode);
            if (parsed == null)
                return (false, "Invalid barcode format. Expected PartNo|Date|Serial");

            bool serialInwarded = await _outwardRepo.IsSerialInInwardAsync(parsed.SerialNumber);
            if (!serialInwarded)
                return (false, $"Serial {parsed.SerialNumber} has no inward record.");

            bool serialOutwarded = await _outwardRepo.IsAlreadyOutAsync(parsed.SerialNumber);
            if (serialOutwarded)
                return (false, $"Serial {parsed.SerialNumber} has already been outwarded.");

            string? defectCode = null;
            if (rawBarcode.Contains("|RW:"))
            {
                defectCode = rawBarcode.Split("|RW:")[1];
            }

            var transaction = new OutwardTransaction
            {
                PartNo = parsed.PartNo,
                SerialNumber = parsed.SerialNumber,
                ScanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ScannedBy = scannedBy,
                DefectCode = defectCode
            };

            await _outwardRepo.InsertAsync(transaction);
            return (true, $"✓ Outward recorded — {parsed.PartNo} | {parsed.SerialNumber}");
        }

        public async Task<(bool ok, string message)> DeleteInwardAsync(int inwardId)
        {
            try
            {
                await _inwardRepo.DeleteAsync(inwardId);
                return (true, "Deleted successfully");
            }
            catch (Exception ex) when (ex.Message.Contains("FOREIGN KEY constraint failed"))
            {
                return (false, "Cannot delete this inward record because it has already been scanned outward. Please delete the outward record first.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting record: {ex.Message}");
            }
        }

        public async Task<(bool ok, string message)> DeleteOutwardAsync(int outwardId)
        {
            try
            {
                await _outwardRepo.DeleteAsync(outwardId);
                return (true, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting record: {ex.Message}");
            }
        }
    }
}
