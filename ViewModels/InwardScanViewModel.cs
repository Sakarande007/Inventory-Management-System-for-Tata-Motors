using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Helpers;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;
using PaintShopIMS.Services;

namespace PaintShopIMS.ViewModels
{
    public partial class InwardScanViewModel : ObservableObject
    {
        private readonly ScanService _scanService;
        private readonly InwardRepository _inwardRepo;
        private readonly DispatcherTimer _clearTimer;

        [ObservableProperty] private string _scanText = string.Empty;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private bool _isError;
        [ObservableProperty] private int _todayCount;

        [ObservableProperty] private string? _scannedQr;

        [ObservableProperty] private bool _isRework;
        partial void OnIsReworkChanged(bool value)
        {
            if (!value && !string.IsNullOrEmpty(ScannedQr))
            {
                // Auto submit when unchecked
                _ = SubmitAsync();
            }
        }

        [ObservableProperty] private string? _selectedDefectCode;
        partial void OnSelectedDefectCodeChanged(string? value)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(ScannedQr) && IsRework)
            {
                // Auto submit when defect selected
                _ = SubmitAsync();
            }
        }
        public Dictionary<string, string> DefectList => DefectCodes.Codes;

        [ObservableProperty] private BitmapImage? _generatedQrImage;
        [ObservableProperty] private bool _hasGeneratedQr;

        [ObservableProperty] private InwardTransaction? _selectedInwardItem;
        public bool IsAdmin => CurrentSession.IsAdmin;

        [ObservableProperty]
        private ObservableCollection<InwardTransaction> _recentScans = new();

        public InwardScanViewModel(ScanService scanService, InwardRepository inwardRepo)
        {
            _scanService = scanService;
            _inwardRepo = inwardRepo;

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
            TodayCount = await _inwardRepo.GetTodayCountAsync();
            var recent = await _inwardRepo.GetRecentAsync(20);
            RecentScans = new ObservableCollection<InwardTransaction>(recent);
        }

        [RelayCommand]
        private void ProcessScan()
        {
            if (string.IsNullOrWhiteSpace(ScanText)) return;

            ScannedQr = ScanText;
            ScanText = string.Empty;

            // Set up UI for rework selection
            IsRework = true;
            SelectedDefectCode = null;
            GeneratedQrImage = null;
            HasGeneratedQr = false;
            
            StatusMessage = "Scan successful. Select defect type or uncheck 'Is Rework' to submit.";
            IsError = false;
            
            _clearTimer.Stop();
            _clearTimer.Start();
        }

        private async Task SubmitAsync()
        {
            if (string.IsNullOrEmpty(ScannedQr)) return;

            string finalQr = ScannedQr;
            string? generatedQr = null;

            if (IsRework)
            {
                if (string.IsNullOrEmpty(SelectedDefectCode)) return;
                
                var cleanRaw = ScannedQr;
                if (cleanRaw.Contains("|RW:")) cleanRaw = cleanRaw.Split("|RW:")[0];
                generatedQr = $"{cleanRaw}|RW:{SelectedDefectCode}";
                finalQr = generatedQr;
            }

            string username = CurrentSession.CurrentUser?.Username ?? "Unknown";
            var result = await _scanService.SaveInwardAsync(ScannedQr, SelectedDefectCode, IsRework, finalQr, username);

            IsError = !result.ok;
            StatusMessage = result.message;

            if (result.ok)
            {
                SoundHelper.PlaySuccess();
                LoadDataAsync();

                if (IsRework && generatedQr != null)
                {
                    GeneratedQrImage = QRGeneratorHelper.GenerateQR(generatedQr);
                    HasGeneratedQr = true;
                }
                else
                {
                    GeneratedQrImage = null;
                    HasGeneratedQr = false;
                }
                
                // Clear ScannedQr so we don't accidentally double-submit
                ScannedQr = null;
            }
            else
            {
                SoundHelper.PlayError();
                GeneratedQrImage = null;
                HasGeneratedQr = false;
            }

            _clearTimer.Stop();
            _clearTimer.Start();
        }

        [RelayCommand]
        public async Task DeleteInward()
        {
            if (SelectedInwardItem == null) return;

            bool confirm = MessageBox.Show(
                "Are you sure you want to delete this inward record?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            ) == MessageBoxResult.Yes;

            if (!confirm) return;

            var result = await _scanService.DeleteInwardAsync(SelectedInwardItem.InwardId);
            if (!result.ok)
            {
                MessageBox.Show(result.message, "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadDataAsync();
        }
    }
}
