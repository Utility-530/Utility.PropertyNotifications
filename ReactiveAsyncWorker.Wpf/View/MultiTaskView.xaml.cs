using ReactiveUI;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for MultiTaskView.xaml
    /// </summary>
    public partial class MultiTaskView
    {
        public MultiTaskView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                ReadyViewModelViewHost.ViewModel = ViewModel.ReadyViewModel;

                RunningViewModelViewHost.ViewModel = ViewModel.RunningViewModel;

                TerminatedViewModelViewHost.ViewModel = ViewModel.TerminatedViewModel;

                CombinedCollectionItemsControl.ItemsSource = ViewModel.CombinedCollection;

                GroupedCollectionItemsControl.ItemsSource = ViewModel.GroupedCollection;
            });
        }
    }
}
