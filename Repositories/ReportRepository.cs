using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PaintShopIMS.Models;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Repository focusing specifically on building reporting aggregates.
    /// </summary>
    public class ReportRepository : BaseRepository
    {
        /// <summary>
        /// Retrieves a unified list of TransactionReports filtering via a specified datarange.
        /// </summary>
        public async Task<IEnumerable<TransactionReport>> GetReportAsync(DateTime from, DateTime to)
        {
            using var db = CreateConnection();
            string query = @"
                SELECT * FROM (
                    SELECT 'INWARD' AS Type, PartNo, SerialNumber, ScanDate, ScannedBy, DefectCode 
                    FROM InwardTransaction 
                    WHERE ScanDate >= @From AND ScanDate <= @To
                    UNION ALL
                    SELECT 'OUTWARD' AS Type, PartNo, SerialNumber, ScanDate, ScannedBy, DefectCode 
                    FROM OutwardTransaction
                    WHERE ScanDate >= @From AND ScanDate <= @To
                )
                ORDER BY ScanDate DESC
            ";
            
            return await db.QueryAsync<TransactionReport>(query, new 
            { 
                From = from.ToString("yyyy-MM-dd 00:00:00"), 
                To = to.ToString("yyyy-MM-dd 23:59:59") 
            });
        }
    }
}
