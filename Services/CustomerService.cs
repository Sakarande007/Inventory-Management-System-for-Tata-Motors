using System.Threading.Tasks;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;

namespace PaintShopIMS.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _customerRepo;

        public CustomerService(CustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<(bool ok, string msg)> AddCustomer(CustomerMaster customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerName))
                return (false, "Customer Name is required.");

            await _customerRepo.InsertAsync(customer);
            return (true, "Customer added successfully.");
        }

        public async Task<(bool ok, string msg)> UpdateCustomer(CustomerMaster customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerName))
                return (false, "Customer Name is required.");

            await _customerRepo.UpdateAsync(customer);
            return (true, "Customer updated successfully.");
        }

        public async Task<(bool ok, string msg)> DeleteCustomer(int customerId)
        {
            await _customerRepo.DeleteAsync(customerId);
            return (true, "Customer deleted successfully.");
        }
    }
}
