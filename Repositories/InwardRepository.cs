using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PaintShopIMS.Models;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Repository for Inward Transaction records.
    /// </summary>
    public class InwardRepository : BaseRepository
    {
        /// <summary>
        /// Checks if a serial number already exists in inward scans.
        /// </summary>
        public async Task<bool> ExistsSerialAsync(string serialNumber)
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM InwardTransaction WHERE SerialNumber = @Serial", new { Serial = serialNumber }) > 0;
        }
        
        /// <summary>
        /// Gets inward scan details by SerialNumber.
        /// </summary>
        public async Task<InwardTransaction?> GetBySerialNumberAsync(string serialNumber)
        {
            using var db = CreateConnection();
            return await db.QuerySingleOrDefaultAsync<InwardTransaction>(
                "SELECT * FROM InwardTransaction WHERE SerialNumber = @Serial", new { Serial = serialNumber });
        }

        /// <summary>
        /// Inserts a new inward transaction.
        /// </summary>
        public async Task InsertAsync(InwardTransaction transaction)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync(
                @"INSERT INTO InwardTransaction (PartNo, SerialNumber, ScanDate, ScannedBy, IsRework, DefectCode, ModifiedQRCode) 
                  VALUES (@PartNo, @SerialNumber, @ScanDate, @ScannedBy, @IsRework, @DefectCode, @ModifiedQRCode)", transaction);
        }

        /// <summary>
        /// Gets the count of inward scans for today.
        /// </summary>
        public async Task<int> GetTodayCountAsync()
        {
            using var db = CreateConnection();
            string today = DateTime.Now.ToString("yyyy-MM-dd") + "%";
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM InwardTransaction WHERE ScanDate LIKE @Today", new { Today = today });
        }

        /// <summary>
        /// Gets inward total stock count.
        /// </summary>
        public async Task<int> GetTotalStockAsync()
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>(
                "SELECT (SELECT COUNT(1) FROM InwardTransaction) - (SELECT COUNT(1) FROM OutwardTransaction)");
        }

        /// <summary>
        /// Gets total rework count.
        /// </summary>
        public async Task<int> GetReworkCountAsync()
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM InwardTransaction WHERE IsRework = 1");
        }

        /// <summary>
        /// Gets inward transactions within a date range.
        /// </summary>
        public async Task<IEnumerable<InwardTransaction>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            using var db = CreateConnection();
            return await db.QueryAsync<InwardTransaction>(
                "SELECT * FROM InwardTransaction WHERE ScanDate >= @From AND ScanDate <= @To ORDER BY InwardId DESC",
                new { From = from.ToString("yyyy-MM-dd 00:00:00"), To = to.ToString("yyyy-MM-dd 23:59:59") });
        }

        /// <summary>
        /// Gets the most recent inward scans.
        /// </summary>
        public async Task<IEnumerable<InwardTransaction>> GetRecentAsync(int count = 20)
        {
            using var db = CreateConnection();
            return await db.QueryAsync<InwardTransaction>(
                "SELECT * FROM InwardTransaction ORDER BY InwardId DESC LIMIT @Count", new { Count = count });
        }

        /// <summary>
        /// Deletes an inward transaction by its ID.
        /// </summary>
        public async Task DeleteAsync(int inwardId)
        {
            using var db = CreateConnection();
            string query = "DELETE FROM InwardTransaction WHERE InwardId = @InwardId";
            await db.ExecuteAsync(query, new { InwardId = inwardId });
        }
    }
}
