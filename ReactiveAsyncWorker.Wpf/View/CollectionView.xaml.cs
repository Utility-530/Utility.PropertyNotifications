using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for CollectionView.xaml
    /// </summary>
    public partial class CollectionView
    {
        public CollectionView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                TitleTextBlock.Text = ViewModel.Name;

                CollectionItemsControl.ItemsSource = this.ViewModel.CollectionTop;

                _ = this.ViewModel.Select(a =>
                  {
                      return this.ViewModel.CollectionAll.Count - this.ViewModel.CollectionTop.Count;
                  }).BindTo(this.RemainingTextBlock, a => a.Text)
            .DisposeWith(disposable);

                _ = this.OneWayBind(this.ViewModel, vm => vm.CollectionAll.Count, v => v.CountTextBlock.Text)
                .DisposeWith(disposable);
            });            
        }
    }
}
