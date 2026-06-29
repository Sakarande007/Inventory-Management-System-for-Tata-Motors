using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintShopIMS.Models;
using PaintShopIMS.Services;

namespace PaintShopIMS.ViewModels
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly ReportService _reportService;

        [ObservableProperty] private DateTime _fromDate = DateTime.Today.AddDays(-7);
        [ObservableProperty] private DateTime _toDate = DateTime.Today;

        [ObservableProperty] private ObservableCollection<TransactionReport> _reportData = new();
        
        [ObservableProperty] private int _totalCount;
        [ObservableProperty] private int _inwardCount;
        [ObservableProperty] private int _outwardCount;
        
        [ObservableProperty] private bool _isBusy;

        public ReportsViewModel(ReportService reportService)
        {
            _reportService = reportService;
            // Load initial default data
            _ = GenerateReportAsync();
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            IsBusy = true;
            try
            {
                var data = await _reportService.GetByDateRange(FromDate, ToDate);
                var list = data.ToList();
                
                ReportData = new ObservableCollection<TransactionReport>(list);
                
                TotalCount = list.Count;
                InwardCount = list.Count(x => x.Type == "INWARD");
                OutwardCount = TotalCount - InwardCount;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ExportToExcelAsync()
        {
            if (ReportData == null || ReportData.Count == 0) return;

            var dialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                FileName = $"Report_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                IsBusy = true;
                
                var (ok, msg) = await _reportService.ExportToExcelAsync(ReportData, dialog.FileName);

                IsBusy = false;

                if (!ok)
                {
                    System.Windows.MessageBox.Show(msg, "Export Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }
    }
}
