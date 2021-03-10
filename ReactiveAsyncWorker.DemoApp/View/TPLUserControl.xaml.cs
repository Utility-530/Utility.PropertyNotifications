using Splat;
using System.Windows.Controls;


namespace DemoApp
{
    /// <summary>
    /// Interaction logic for TPLViewModel.xaml
    /// </summary>
    public partial class TPLUserControl : UserControl
    {
        public TPLUserControl()
        {
            InitializeComponent();

            var masterTPLViewModel = Locator.Current.GetService<MasterTPLViewModel>();

            MainViewModelViewHost.ViewModel = masterTPLViewModel.MainViewModel.Service;

            CollectionModelViewHost.ViewModel = masterTPLViewModel.MainViewModel.Collection;

            ItemsControl1.ItemsSource = masterTPLViewModel.OutputModel.Collection;

            this.DataContext = masterTPLViewModel;
        }
    }
}
