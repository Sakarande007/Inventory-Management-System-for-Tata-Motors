using System.Threading.Tasks;
using Dapper;
using PaintShopIMS.Models;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Repository for managing User logins and access.
    /// </summary>
    public class UserRepository : BaseRepository
    {
        /// <summary>
        /// Looks up a user completely via Username.
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var db = CreateConnection();
            return await db.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username", new { Username = username });
        }
    }
}
