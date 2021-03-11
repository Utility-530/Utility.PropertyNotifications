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

                CombinedViewModelViewHost.ViewModel = ViewModel.CombinedCollection;

                GroupedViewModelViewHost.ViewModel = ViewModel.GroupedCollection;

                LatestViewModelViewHost.ViewModel = ViewModel.LatestViewModel;
            });
        }
    }
}
