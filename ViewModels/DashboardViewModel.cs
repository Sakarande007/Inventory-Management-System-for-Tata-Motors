using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintShopIMS.Models;
using PaintShopIMS.Services;

namespace PaintShopIMS.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DashboardService _dashboardService;

        [ObservableProperty] private int _todayInward;
        [ObservableProperty] private int _todayOutward;
        [ObservableProperty] private int _totalStock;
        [ObservableProperty] private int _reworkCount;

        [ObservableProperty]
        private ObservableCollection<TransactionReport> _recentTransactions = new();

        public DashboardViewModel(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task LoadAsync()
        {
            var stats = await _dashboardService.GetStats();
            TodayInward = stats.TodayInward;
            TodayOutward = stats.TodayOutward;
            TotalStock = stats.TotalStock;
            ReworkCount = stats.ReworkCount;

            var recent = await _dashboardService.GetRecentTransactions(8);
            RecentTransactions = new ObservableCollection<TransactionReport>(recent);
        }
    }
}
