using System;
using System.Windows;
using System.Windows.Threading;
using PaintShopIMS.ViewModels;
using Microsoft.Extensions.DependencyInjection; // Assumes using dependency injection in App.xaml.cs

namespace PaintShopIMS.Views
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Trigger animation on load
            contentControl.SetBinding(System.Windows.Controls.ContentControl.ContentProperty, 
                new System.Windows.Data.Binding("CurrentView") { NotifyOnTargetUpdated = true });

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) =>
            {
                txtLiveClock.Text = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
            };
            _timer.Start();

            this.Closing += (s, e) => _timer.Stop();
            this.Loaded += (s, e) => txtLiveClock.Text = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
        }
    }
}
