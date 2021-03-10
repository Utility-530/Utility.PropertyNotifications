using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                this.OneWayBind(this.ViewModel, vm => vm.IsFree, v => v.IsFreeTextBlock).DisposeWith(diposable);

                this.OneWayBind(this.ViewModel, vm => vm.Count, v => v.CountTextBlock).DisposeWith(diposable);

            });
        }
    }
}
