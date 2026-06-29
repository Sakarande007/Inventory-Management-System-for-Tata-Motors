using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;
using PaintShopIMS.Services;
using PaintShopIMS.Views.Customers;

namespace PaintShopIMS.ViewModels
{
    public partial class CustomerMasterViewModel : ObservableObject
    {
        private readonly CustomerRepository _customerRepo;
        private readonly CustomerService _customerService;

        private List<CustomerMaster> _allCustomers = new();

        [ObservableProperty] private ObservableCollection<CustomerMaster> _filteredCustomers = new();
        [ObservableProperty] private CustomerMaster? _selectedCustomer;
        
        [ObservableProperty] private string _searchText = string.Empty;
        
        public int CustomersCount => FilteredCustomers.Count;

        public CustomerMasterViewModel(CustomerRepository customerRepo, CustomerService customerService)
        {
            _customerRepo = customerRepo;
            _customerService = customerService;
            LoadDataAsync();
        }

        public async void LoadDataAsync()
        {
            var customers = await _customerRepo.GetAllAsync();
            _allCustomers = customers.ToList();
            FilterList();
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterList();
        }

        private void FilterList()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredCustomers = new ObservableCollection<CustomerMaster>(_allCustomers);
            }
            else
            {
                var lower = SearchText.ToLower();
                var filtered = _allCustomers.Where(c => 
                    c.CustomerName.ToLower().Contains(lower) || 
                    (c.Description != null && c.Description.ToLower().Contains(lower))
                ).ToList();
                FilteredCustomers = new ObservableCollection<CustomerMaster>(filtered);
            }
            OnPropertyChanged(nameof(CustomersCount));
        }

        [RelayCommand]
        private void AddCustomer()
        {
            var vm = new CustomerEditDialogViewModel(_customerService, new CustomerMaster());
            var dialog = new CustomerEditDialog(vm);
            
            if (dialog.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        [RelayCommand]
        private void EditCustomer()
        {
            if (SelectedCustomer == null) return;

            var editClone = new CustomerMaster
            {
                CustomerId = SelectedCustomer.CustomerId,
                CustomerName = SelectedCustomer.CustomerName,
                Description = SelectedCustomer.Description,
                Remark = SelectedCustomer.Remark
            };

            var vm = new CustomerEditDialogViewModel(_customerService, editClone);
            var dialog = new CustomerEditDialog(vm);

            if (dialog.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteCustomerAsync()
        {
            if (SelectedCustomer == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedCustomer.CustomerName}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                var response = await _customerService.DeleteCustomer(SelectedCustomer.CustomerId);
                if (response.ok)
                {
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(response.msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
