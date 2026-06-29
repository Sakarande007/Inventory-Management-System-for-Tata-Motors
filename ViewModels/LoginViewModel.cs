using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;
using PaintShopIMS.Helpers;

namespace PaintShopIMS.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly UserRepository _userRepository;

        [ObservableProperty] private string _username = "";
        [ObservableProperty] private string _password = "";
        [ObservableProperty] private string _errorMessage = "";

        // Event back to the UI to handle window transition
        public event EventHandler? LoginSuccessful;

        public LoginViewModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                string cleanUsername = Username?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(cleanUsername) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter both username and password.";
                    return;
                }

                var user = await _userRepository.GetByUsernameAsync(cleanUsername);
                if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
                {
                    ErrorMessage = "Invalid username or password.";
                    return;
                }

                // Successful login
                CurrentSession.SetUser(user);
                ErrorMessage = "";
                
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Database connection failed. Check debug popup.";
                System.Windows.MessageBox.Show(ex.ToString(), "Application Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
