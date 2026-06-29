using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;

namespace PaintShopIMS.Services
{
    public class ReportService
    {
        private readonly ReportRepository _reportRepo;

        public ReportService(ReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }

        public async Task<IEnumerable<TransactionReport>> GetByDateRange(DateTime from, DateTime to)
        {
            return await _reportRepo.GetReportAsync(from, to);
        }

        public async Task<(bool ok, string msg)> ExportToExcelAsync(IEnumerable<TransactionReport> data, string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("Transaction Report");

                        // Headers
                        var headers = new[] { "Type", "Part No", "Description", "Serial Number", "Scan Date", "Scanned By", "Remark", "Rework Code" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            ws.Cell(1, i + 1).Value = headers[i];
                            var headerCell = ws.Cell(1, i + 1);
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.Font.FontColor = XLColor.White;
                            headerCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F1C3A"); // NavyDark
                        }

                        // Data
                        int row = 2;
                        foreach (var item in data)
                        {
                            ws.Cell(row, 1).Value = item.Type;
                            ws.Cell(row, 2).Value = item.PartNo;
                            ws.Cell(row, 3).Value = item.Description;
                            ws.Cell(row, 4).Value = item.SerialNumber;
                            
                            ws.Cell(row, 5).Value = item.ScanDate;
                            ws.Cell(row, 5).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";

                            ws.Cell(row, 6).Value = item.ScannedBy;
                            ws.Cell(row, 7).Value = item.Remark;
                            ws.Cell(row, 8).Value = item.DefectCode;

                            // Light row colors
                            var color = item.Type == "INWARD" ? XLColor.FromHtml("#EAF3DE") : XLColor.FromHtml("#FAEEDA");
                            ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = color;

                            row++;
                        }

                        // Auto-fit
                        ws.Columns().AdjustToContents();

                        workbook.SaveAs(filePath);
                    }

                    // Open file
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                    return (true, "Exported successfully.");
                });
            }
            catch (Exception ex)
            {
                return (false, $"Export failed: {ex.Message}");
            }
        }
    }
}
