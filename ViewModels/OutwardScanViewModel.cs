using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Helpers;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;
using PaintShopIMS.Services;

namespace PaintShopIMS.ViewModels
{
    public partial class OutwardScanViewModel : ObservableObject
    {
        private readonly ScanService _scanService;
        private readonly OutwardRepository _outwardRepo;
        private readonly DispatcherTimer _clearTimer;

        [ObservableProperty] private string _scanText = string.Empty;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private bool _isError;
        [ObservableProperty] private int _todayCount;

        [ObservableProperty] private OutwardTransaction? _selectedOutwardItem;
        public bool IsAdmin => CurrentSession.IsAdmin;

        [ObservableProperty]
        private ObservableCollection<OutwardTransaction> _recentScans = new();

        public OutwardScanViewModel(ScanService scanService, OutwardRepository outwardRepo)
        {
            _scanService = scanService;
            _outwardRepo = outwardRepo;

            _clearTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _clearTimer.Tick += (s, e) =>
            {
                StatusMessage = string.Empty;
                _clearTimer.Stop();
            };
            
            LoadDataAsync();
        }

        public async void LoadDataAsync()
        {
            TodayCount = await _outwardRepo.GetTodayCountAsync();
            var recent = await _outwardRepo.GetRecentAsync(20);
            RecentScans = new ObservableCollection<OutwardTransaction>(recent);
        }

        [RelayCommand]
        private async Task ProcessScanAsync()
        {
            if (string.IsNullOrWhiteSpace(ScanText)) return;

            string rawScan = ScanText;
            ScanText = string.Empty;

            string username = CurrentSession.CurrentUser?.Username ?? "Unknown";
            var result = await _scanService.ProcessOutward(rawScan, username);

            IsError = !result.ok;
            StatusMessage = result.message;

            if (result.ok)
            {
                SoundHelper.PlaySuccess();
                LoadDataAsync();
            }
            else
            {
                SoundHelper.PlayError();
            }

            _clearTimer.Stop();
            _clearTimer.Start();
        }

        [RelayCommand]
        public async Task DeleteOutward()
        {
            if (SelectedOutwardItem == null) return;

            bool confirm = MessageBox.Show(
                "Are you sure you want to delete this outward record?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            ) == MessageBoxResult.Yes;

            if (!confirm) return;

            var result = await _scanService.DeleteOutwardAsync(SelectedOutwardItem.OutwardId);
            if (!result.ok)
            {
                MessageBox.Show(result.message, "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadDataAsync();
        }
    }
}
