using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using PaintShopIMS.ViewModels;

namespace PaintShopIMS.Views.Scan
{
    public partial class OutwardScanView : UserControl
    {
        private readonly Storyboard _slideIn;

        public OutwardScanView()
        {
            InitializeComponent();
            _slideIn = BuildSlideInAnimation();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is OutwardScanViewModel oldVm)
                oldVm.PropertyChanged -= OnVmPropertyChanged;

            if (e.NewValue is OutwardScanViewModel newVm)
                newVm.PropertyChanged += OnVmPropertyChanged;
        }

        private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OutwardScanViewModel.StatusMessage))
            {
                if (sender is OutwardScanViewModel vm && !string.IsNullOrEmpty(vm.StatusMessage))
                    _slideIn.Begin(lblStatus, true);
            }
        }

        private Storyboard BuildSlideInAnimation()
        {
            var sb = new Storyboard();
            var t = new DoubleAnimation { From = -20, To = 0, Duration = new System.Windows.Duration(System.TimeSpan.FromSeconds(0.15)) };
            Storyboard.SetTargetProperty(t, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            var o = new DoubleAnimation { From = 0, To = 1, Duration = new System.Windows.Duration(System.TimeSpan.FromSeconds(0.15)) };
            Storyboard.SetTargetProperty(o, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(t);
            sb.Children.Add(o);
            return sb;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => _txtScan.Focus();

        private void TxtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is OutwardScanViewModel vm)
                if (vm.ProcessScanCommand.CanExecute(null))
                    vm.ProcessScanCommand.Execute(null);
        }
    }
}
