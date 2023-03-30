using System.Windows.Controls;
using Utility.Common;
using Utility.WPF.Demo.Common.Meta;

namespace UtilityWpf.Demo.Hybrid
{
    /// <summary>
    /// Interaction logic for FactoryLogUserControl.xaml
    /// </summary>
    public partial class FactoryLogUserControl : UserControl
    {
        public FactoryLogUserControl()
        {
            InitializeComponent();
            MainDataGrid.ItemsSource =Resolver.Instance.Resolve<FactoryLogger>().Logs;
        }
    }
}