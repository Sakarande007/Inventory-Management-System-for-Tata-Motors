using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PaintShopIMS.Models;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Repository for managing PartMaster records.
    /// </summary>
    public class PartRepository : BaseRepository
    {
        /// <summary>
        /// Gets all parts.
        /// </summary>
        public async Task<IEnumerable<PartMaster>> GetAllAsync()
        {
            using var db = CreateConnection();
            return await db.QueryAsync<PartMaster>("SELECT * FROM PartMaster");
        }

        /// <summary>
        /// Gets a part by its specific Number.
        /// </summary>
        public async Task<PartMaster?> GetByPartNoAsync(string partNo)
        {
            using var db = CreateConnection();
            return await db.QuerySingleOrDefaultAsync<PartMaster>(
                "SELECT * FROM PartMaster WHERE PartNo = @PartNo", new { PartNo = partNo });
        }

        /// <summary>
        /// Checks if a part number already exists.
        /// </summary>
        public async Task<bool> ExistsPartNoAsync(string partNo)
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM PartMaster WHERE PartNo = @PartNo", new { PartNo = partNo }) > 0;
        }

        /// <summary>
        /// Checks if a part number exists excluding a specific ID.
        /// </summary>
        public async Task<bool> ExistsPartNoExcludingAsync(string partNo, int excludeId)
        {
            using var db = CreateConnection();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM PartMaster WHERE PartNo = @PartNo AND PartId != @ExcludeId",
                new { PartNo = partNo, ExcludeId = excludeId }) > 0;
        }

        /// <summary>
        /// Inserts a new PartMaster record.
        /// </summary>
        public async Task InsertAsync(PartMaster part)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync(
                @"INSERT INTO PartMaster (PartNo, PartName, Description, Remark) 
                  VALUES (@PartNo, @PartName, @Description, @Remark)", part);
        }

        /// <summary>
        /// Updates an existing PartMaster record.
        /// </summary>
        public async Task UpdateAsync(PartMaster part)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync(
                @"UPDATE PartMaster 
                  SET PartName = @PartName, Description = @Description, Remark = @Remark 
                  WHERE PartId = @PartId OR PartNo = @PartNo", part);
        }

        /// <summary>
        /// Deletes a part by its internal ID.
        /// </summary>
        public async Task DeleteAsync(int partId)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync("DELETE FROM PartMaster WHERE PartId = @PartId", new { PartId = partId });
        }
    }
}
