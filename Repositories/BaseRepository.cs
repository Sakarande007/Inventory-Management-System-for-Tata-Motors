using System.Data;
using Microsoft.Data.Sqlite;
using PaintShopIMS.Data;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Base repository providing a method to acquire database connections.
    /// </summary>
    public abstract class BaseRepository
    {
        /// <summary>
        /// Creates and returns a new configured SQLite connection.
        /// </summary>
        protected IDbConnection CreateConnection()
        {
            return new SqliteConnection(AppConfig.ConnectionString);
        }
    }
}
