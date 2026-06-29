using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using PaintShopIMS.ViewModels;

namespace PaintShopIMS.Views.Dashboard
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DashboardViewModel vm)
            {
                await vm.LoadAsync();
                AnimateNumber(valTodayInward, vm.TodayInward);
                AnimateNumber(valTodayOutward, vm.TodayOutward);
                AnimateNumber(valTotalStock, vm.TotalStock);
            }
        }

        private void AnimateNumber(TextBlock target, int finalValue)
        {
            if (finalValue == 0) return;
            
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = finalValue,
                Duration = new Duration(TimeSpan.FromMilliseconds(600))
            };

            // Using code-behind animation for number counting
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            Storyboard.SetTarget(animation, target);
            
            // WPF cannot directly animate string / int text blocks, so we animate a custom property or format in tick.
            // Using a dispatcher timer is much easier to animate text values smoothly.
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // ~60fps
            double current = 0;
            double step = finalValue / (600.0 / 16.0);
            
            timer.Tick += (s, args) =>
            {
                current += step;
                if (current >= finalValue)
                {
                    current = finalValue;
                    timer.Stop();
                }
                target.Text = ((int)current).ToString("N0");
            };
            timer.Start();
        }
    }
}
