using ReactiveUI;
using System.Reactive.Disposables;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for AsyncTaskStatusView.xaml
    /// </summary>
    public partial class AsyncTaskStatusView
    {
        public AsyncTaskStatusView()
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
