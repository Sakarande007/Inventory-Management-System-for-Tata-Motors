using System.Threading.Tasks;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;
using System.Linq;

namespace PaintShopIMS.Services
{
    public class DashboardService
    {
        private readonly DashboardRepository _dashboardRepo;
        private readonly InwardRepository _inwardRepo;
        private readonly OutwardRepository _outwardRepo;

        public DashboardService(DashboardRepository dashboardRepo, InwardRepository inwardRepo, OutwardRepository outwardRepo)
        {
            _dashboardRepo = dashboardRepo;
            _inwardRepo = inwardRepo;
            _outwardRepo = outwardRepo;
        }

        public async Task<DashboardStats> GetStats()
        {
            var inward = await _inwardRepo.GetTodayCountAsync();
            var outward = await _outwardRepo.GetTodayCountAsync();
            var stock = await _dashboardRepo.GetTotalStockAsync();
            var rework = await _inwardRepo.GetReworkCountAsync();

            return new DashboardStats
            {
                TodayInward = inward,
                TodayOutward = outward,
                TotalStock = stock,
                ReworkCount = rework
            };
        }

        public async Task<System.Collections.Generic.IEnumerable<TransactionReport>> GetRecentTransactions(int count = 8)
        {
            return await _dashboardRepo.GetRecentTransactionsAsync(count);
        }
    }
}
