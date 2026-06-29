namespace PaintShopIMS.Models
{
    /// <summary>
    /// Aggregated statistics for the dashboard view.
    /// </summary>
    public class DashboardStats
    {
        public int TodayInward { get; set; }
        public int TodayOutward { get; set; }
        public int TotalStock { get; set; }
        public int ReworkCount { get; set; }
    }
}
