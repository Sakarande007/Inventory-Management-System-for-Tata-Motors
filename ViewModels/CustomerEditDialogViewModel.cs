using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Models;
using PaintShopIMS.Services;

namespace PaintShopIMS.ViewModels
{
    public partial class CustomerEditDialogViewModel : ObservableObject
    {
        private readonly CustomerService _customerService;
        private readonly CustomerMaster _customer;

        public bool IsNew => _customer.CustomerId == 0;
        public string Title => IsNew ? "Add Customer" : "Edit Customer";

        [ObservableProperty] private string _customerName = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _remark = string.Empty;

        [ObservableProperty] private Dictionary<string, string> _validationErrors = new();

        public CustomerEditDialogViewModel(CustomerService customerService, CustomerMaster customer)
        {
            _customerService = customerService;
            _customer = customer;

            CustomerName = customer.CustomerName ?? string.Empty;
            Description = customer.Description ?? string.Empty;
            Remark = customer.Remark ?? string.Empty;
        }

        [RelayCommand]
        private async Task SaveAsync(Window window)
        {
            ValidationErrors.Clear();

            if (string.IsNullOrWhiteSpace(CustomerName))
                ValidationErrors["CustomerName"] = "Customer Name is required";

            if (ValidationErrors.Count > 0)
            {
                OnPropertyChanged(nameof(ValidationErrors));
                return;
            }

            _customer.CustomerName = CustomerName;
            _customer.Description = Description;
            _customer.Remark = Remark;

            var (ok, msg) = IsNew 
                ? await _customerService.AddCustomer(_customer) 
                : await _customerService.UpdateCustomer(_customer);

            if (ok)
            {
                window.DialogResult = true;
                window.Close();
            }
            else
            {
                MessageBox.Show(msg, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
