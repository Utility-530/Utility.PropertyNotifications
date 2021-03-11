using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for LimitCollectionView.xaml
    /// </summary>
    public partial class LimitCollectionView 
    {
        public LimitCollectionView()
        {
            InitializeComponent();
            this.WhenActivated(diposable =>
            {
                this.ViewModel.WhenAnyValue(a => a.IsFree)
                .DistinctUntilChanged()
                .Subscribe(c =>
                {
                    var text = c.ToString();
                    RxApp.MainThreadScheduler.Schedule(Unit.Default, (a, b) =>
                    {
                        IsFreeTextBlock.Text = text;
                        return Disposable.Empty;
                    });
                });

                // This doesn't work
                //this.OneWayBind(this.ViewModel, vm => vm.IsFree, v => v.IsFreeTextBlock.Text, a=>a.ToString()).DisposeWith(diposable);

                this.OneWayBind(this.ViewModel, vm => vm.Count, v => v.CountTextBlock.Text).DisposeWith(diposable);

            });
        }
    }
}
