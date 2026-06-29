using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PaintShopIMS.ViewModels;

namespace PaintShopIMS.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var vm = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<LoginViewModel>(App.ServiceProvider);
            DataContext = vm;
            
            vm.LoginSuccessful += OnLoginSuccessful;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = txtPassword.Password;
            }
        }

        private void OnLoginSuccessful(object? sender, EventArgs e)
        {
            // Detach event to prevent memory leaks if recreated
            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSuccessful -= OnLoginSuccessful;
            }
            
            var mainWindow = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainWindow>(App.ServiceProvider);
            mainWindow.Show();
            
            this.Close();
        }
    }
}
