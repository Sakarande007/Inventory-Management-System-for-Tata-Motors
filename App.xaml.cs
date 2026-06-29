using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PaintShopIMS.Helpers;
using PaintShopIMS.Repositories;
using PaintShopIMS.Services;
using PaintShopIMS.ViewModels;
using PaintShopIMS.Views;
using PaintShopIMS.Views.Dashboard;
using PaintShopIMS.Views.Parts;
using PaintShopIMS.Views.Customers;
using PaintShopIMS.Views.Reports;
using PaintShopIMS.Views.Scan;

namespace PaintShopIMS
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            SystemSetupHelper.Initialize();
            base.OnStartup(e);

            // Global Exception Handling
            this.DispatcherUnhandledException += (s, args) => LogException(args.Exception, "Dispatcher");
            AppDomain.CurrentDomain.UnhandledException += (s, args) => LogException(args.ExceptionObject as Exception, "AppDomain");
            TaskScheduler.UnobservedTaskException += (s, args) => LogException(args.Exception, "TaskScheduler");

            PaintShopIMS.Data.DatabaseInitializer.Initialize();
            
            // Execute Diagnostic tool
            Task.Run(async () => await TestDb.SeedAndTestAsync()).Wait();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void LogException(Exception? ex, string source)
        {
            if (ex == null) return;

            string logPath = "CRASH_LOG.txt";
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR ({source}): {ex.Message}\n{ex.StackTrace}\n";
            if (ex.InnerException != null)
            {
                logEntry += $"INNER EXCEPTION: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}\n";
            }
            logEntry += "-------------------------------------------------------\n";

            try
            {
                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch { /* Avoid crashing the crash logger */ }

            MessageBox.Show($"A critical error occurred ({source}). Details saved to CRASH_LOG.txt\n\nError: {ex.Message}", "Application Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Repositories
            services.AddSingleton<UserRepository>();
            services.AddSingleton<PartRepository>();
            services.AddSingleton<CustomerRepository>();
            services.AddSingleton<InwardRepository>();
            services.AddSingleton<OutwardRepository>();
            services.AddSingleton<DashboardRepository>();
            services.AddSingleton<ReportRepository>();

            // Services
            services.AddSingleton<ScanService>();
            services.AddSingleton<PartService>();
            services.AddSingleton<CustomerService>();
            services.AddSingleton<DashboardService>();
            services.AddSingleton<ReportService>();

            // View Models
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<InwardScanViewModel>();
            services.AddSingleton<OutwardScanViewModel>();
            services.AddSingleton<PartMasterViewModel>();
            services.AddSingleton<CustomerMasterViewModel>();
            services.AddSingleton<ReportsViewModel>();
            
            // Note: Dialog viewmodels are structurally created transiently on Edit, but we can register if needed.
            
            // Views
            services.AddTransient<LoginWindow>();
            services.AddTransient<PaintShopIMS.Views.MainWindow>();
        }
    }
}
