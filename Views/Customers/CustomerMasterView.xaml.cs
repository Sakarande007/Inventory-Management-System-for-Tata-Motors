using System.Windows.Controls;
using System.Windows.Input;
using PaintShopIMS.ViewModels;

namespace PaintShopIMS.Views.Customers
{
    public partial class CustomerMasterView : UserControl
    {
        public CustomerMasterView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is CustomerMasterViewModel vm && vm.EditCustomerCommand.CanExecute(null))
            {
                vm.EditCustomerCommand.Execute(null);
            }
        }
    }
}
