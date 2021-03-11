using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using System.Windows;

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

                var remaining = this.ViewModel.Select(a =>
                {
                    return this.ViewModel.CollectionAll.Count - this.ViewModel.CollectionTop.Count;
                });

                _ = remaining
                    .BindTo(this.RemainingTextBlock, a => a.Text)
                    .DisposeWith(disposable);

                remaining
                .Select(a => a > 0 ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(this.RemainingTextBlock, a => a.Visibility)
                .DisposeWith(disposable);

                _ = this.OneWayBind(this.ViewModel, vm => vm.CollectionAll.Count, v => v.CountTextBlock.Text)
                    .DisposeWith(disposable);
            });
        }
    }
}
