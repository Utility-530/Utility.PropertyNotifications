using System.Windows.Controls;
using Utility.WPF.Controls.Master;
using Utility.WPF.Demo.Master.ViewModels;
using RowViewModel = Utility.WPF.Demo.Master.ViewModels.RowViewModel;

namespace Utility.WPF.Demo.Master.Controls
{
    public class AddRowControl : MasterControl
    {
        protected override void ExecuteAdd()
        {
            ((Content as ListBox).DataContext as MainViewModel).Rows.Add(new RowViewModel());
        }
    }
}