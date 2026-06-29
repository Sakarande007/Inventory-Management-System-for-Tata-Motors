using System.Windows;
using PaintShopIMS.ViewModels;

namespace PaintShopIMS.Views.Customers
{
    public partial class CustomerEditDialog : Window
    {
        public CustomerEditDialog(CustomerEditDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
