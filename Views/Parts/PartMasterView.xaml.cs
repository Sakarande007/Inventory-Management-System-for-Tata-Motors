using System.Windows.Controls;
using System.Windows.Input;
using PaintShopIMS.ViewModels;

namespace PaintShopIMS.Views.Parts
{
    public partial class PartMasterView : UserControl
    {
        public PartMasterView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is PartMasterViewModel vm && vm.EditPartCommand.CanExecute(null))
            {
                vm.EditPartCommand.Execute(null);
            }
        }
    }
}
