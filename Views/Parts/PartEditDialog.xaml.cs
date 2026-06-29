using System.Windows;
using PaintShopIMS.ViewModels;

namespace PaintShopIMS.Views.Parts
{
    public partial class PartEditDialog : Window
    {
        public PartEditDialog(PartEditDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
