using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PaintShopIMS.Models;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Repository to fetch unified dashboard data.
    /// </summary>
    public class DashboardRepository : BaseRepository
    {
        /// <summary>
        /// Gets the total active stock (Inward - Outward)
        /// </summary>
        public async Task<int> GetTotalStockAsync()
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>(
                "SELECT (SELECT COUNT(1) FROM InwardTransaction) - (SELECT COUNT(1) FROM OutwardTransaction)");
        }

        /// <summary>
        /// Gets unified recent transactions combining Inwards and Outwards.
        /// </summary>
        public async Task<IEnumerable<TransactionReport>> GetRecentTransactionsAsync(int count = 8)
        {
            using var db = CreateConnection();
            string query = @"
                SELECT 'INWARD' AS Type, PartNo, SerialNumber, ScanDate, ScannedBy, DefectCode 
                FROM InwardTransaction
                UNION ALL
                SELECT 'OUTWARD' AS Type, PartNo, SerialNumber, ScanDate, ScannedBy, DefectCode 
                FROM OutwardTransaction
                ORDER BY ScanDate DESC
                LIMIT @Count
            ";
            
            return await db.QueryAsync<TransactionReport>(query, new { Count = count });
        }
    }
}
