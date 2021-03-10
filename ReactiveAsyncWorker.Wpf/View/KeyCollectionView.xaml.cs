using ReactiveUI;
using System.Reactive.Disposables;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for AsyncTaskStatusView.xaml
    /// </summary>
    public partial class KeyCollectionView 
    {
        public KeyCollectionView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {

                this.OneWayBind(this.ViewModel, vm => vm.Key, v => v.KeyTextBlock.Text)
                .DisposeWith(disposable);

                this.OneWayBind(this.ViewModel, vm => vm.Collection, v => v.CollectionItemsControl.ItemsSource)
                .DisposeWith(disposable);

            });
        }
    }
}
