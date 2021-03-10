using ReactiveUI;
using System.Reactive.Disposables;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for AsyncTaskStatusView.xaml
    /// </summary>
    public partial class ReactiveProcessPairView
    {
        public ReactiveProcessPairView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                KeyTextBlock.Text = ViewModel.Key.ToString();
                ValueItemsControl.ItemsSource = ViewModel.Value;
            });
        }
    }
}
