using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Services;
using Microsoft.Extensions.DependencyInjection; // Assumes App.ServiceProvider usage
using PaintShopIMS.Helpers;

namespace PaintShopIMS.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableObject? _currentView;

        [ObservableProperty]
        private string _currentPageTitle = "Dashboard";

        public bool IsAdmin => CurrentSession.IsAdmin;
        public string Username => CurrentSession.CurrentUser?.Username ?? "Guest";

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            // Default View
            NavigateTo("Dashboard");
        }

        [RelayCommand]
        private void NavigateTo(string pageName)
        {
            switch (pageName)
            {
                case "Dashboard":
                    CurrentView = _serviceProvider.GetRequiredService<DashboardViewModel>();
                    CurrentPageTitle = "Dashboard";
                    break;
                case "Inward":
                    CurrentView = _serviceProvider.GetRequiredService<InwardScanViewModel>();
                    CurrentPageTitle = "Inward Scan";
                    break;
                case "Outward":
                    CurrentView = _serviceProvider.GetRequiredService<OutwardScanViewModel>();
                    CurrentPageTitle = "Outward Scan";
                    break;
                case "Parts":
                    CurrentView = _serviceProvider.GetRequiredService<PartMasterViewModel>();
                    CurrentPageTitle = "Part Master";
                    break;
                case "Customers":
                    CurrentView = _serviceProvider.GetRequiredService<CustomerMasterViewModel>();
                    CurrentPageTitle = "Customer Master";
                    break;
                case "Reports":
                    CurrentView = _serviceProvider.GetRequiredService<ReportsViewModel>();
                    CurrentPageTitle = "Reports";
                    break;
            }
        }

        [RelayCommand]
        private void Logout()
        {
            CurrentSession.Clear();
            
            // Open Login Window
            var loginWindow = App.ServiceProvider.GetRequiredService<Views.LoginWindow>();
            loginWindow.Show();
            
            // Close Main Window
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Views.MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
