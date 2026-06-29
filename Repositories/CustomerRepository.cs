using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PaintShopIMS.Models;

namespace PaintShopIMS.Repositories
{
    /// <summary>
    /// Repository for managing CustomerMaster records.
    /// </summary>
    public class CustomerRepository : BaseRepository
    {
        /// <summary>
        /// Gets all customers.
        /// </summary>
        public async Task<IEnumerable<CustomerMaster>> GetAllAsync()
        {
            using var db = CreateConnection();
            return await db.QueryAsync<CustomerMaster>("SELECT * FROM CustomerMaster");
        }

        /// <summary>
        /// Inserts a new customer record.
        /// </summary>
        public async Task InsertAsync(CustomerMaster customer)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync(
                @"INSERT INTO CustomerMaster (CustomerName, Description, Remark) 
                  VALUES (@CustomerName, @Description, @Remark)", customer);
        }

        /// <summary>
        /// Updates an existing customer record.
        /// </summary>
        public async Task UpdateAsync(CustomerMaster customer)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync(
                @"UPDATE CustomerMaster 
                  SET CustomerName = @CustomerName, Description = @Description, Remark = @Remark 
                  WHERE CustomerId = @CustomerId", customer);
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// </summary>
        public async Task DeleteAsync(int customerId)
        {
            using var db = CreateConnection();
            await db.ExecuteAsync("DELETE FROM CustomerMaster WHERE CustomerId = @CustomerId", new { CustomerId = customerId });
        }
    }
}
