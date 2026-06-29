using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PaintShopIMS.Models;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Repository for Outward Transaction records.
    /// </summary>
    public class OutwardRepository : BaseRepository
    {
        /// <summary>
        /// Checks if the serial number was inward scanned.
        /// </summary>
        public async Task<bool> IsSerialInInwardAsync(string serialNumber)
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM InwardTransaction WHERE SerialNumber = @Serial", new { Serial = serialNumber }) > 0;
        }

        /// <summary>
        /// Checks if the serial number is already outward scanned.
        /// </summary>
        public async Task<bool> IsAlreadyOutAsync(string serialNumber)
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM OutwardTransaction WHERE SerialNumber = @Serial", new { Serial = serialNumber }) > 0;
        }
        
        /// <summary>
        /// Gets outward scan details by SerialNumber.
        /// </summary>
        public async Task<OutwardTransaction?> GetBySerialNumberAsync(string serialNumber)
        {
            using var db = CreateConnection();
            return await db.QuerySingleOrDefaultAsync<OutwardTransaction>(
                "SELECT * FROM OutwardTransaction WHERE SerialNumber = @Serial", new { Serial = serialNumber });
        }

        /// <summary>
        /// Inserts a new outward transaction.
        /// </summary>
        public async Task InsertAsync(OutwardTransaction transaction)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync(
                @"INSERT INTO OutwardTransaction (PartNo, SerialNumber, ScanDate, ScannedBy, DefectCode) 
                  VALUES (@PartNo, @SerialNumber, @ScanDate, @ScannedBy, @DefectCode)", transaction);
        }

        /// <summary>
        /// Gets the count of outward scans for today.
        /// </summary>
        public async Task<int> GetTodayCountAsync()
        {
            using var db = CreateConnection();
            string today = DateTime.Now.ToString("yyyy-MM-dd") + "%";
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM OutwardTransaction WHERE ScanDate LIKE @Today", new { Today = today });
        }

        /// <summary>
        /// Gets outward transactions within a date range.
        /// </summary>
        public async Task<IEnumerable<OutwardTransaction>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            using var db = CreateConnection();
            return await db.QueryAsync<OutwardTransaction>(
                "SELECT * FROM OutwardTransaction WHERE ScanDate >= @From AND ScanDate <= @To ORDER BY OutwardId DESC",
                new { From = from.ToString("yyyy-MM-dd 00:00:00"), To = to.ToString("yyyy-MM-dd 23:59:59") });
        }

        /// <summary>
        /// Gets the most recent outward scans.
        /// </summary>
        public async Task<IEnumerable<OutwardTransaction>> GetRecentAsync(int count = 20)
        {
            using var db = CreateConnection();
            return await db.QueryAsync<OutwardTransaction>(
                "SELECT * FROM OutwardTransaction ORDER BY OutwardId DESC LIMIT @Count", new { Count = count });
        }

        /// <summary>
        /// Deletes an outward transaction by its ID.
        /// </summary>
        public async Task DeleteAsync(int outwardId)
        {
            using var db = CreateConnection();
            string query = "DELETE FROM OutwardTransaction WHERE OutwardId = @OutwardId";
            await db.ExecuteAsync(query, new { OutwardId = outwardId });
        }
    }
}
