using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Controls;
using DynamicData;
using Utility.WPF.Demo.Data.Model;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for TreeViewUserControl.xaml
    /// </summary>
    public partial class TreeViewUserControl : UserControl
    {
        private Stock stock;
        private ReadOnlyObservableCollection<Sector> sectors;

        public ReadOnlyObservableCollection<Sector> Sectors => sectors;

        public Stock Stock
        {
            get => stock;
            set => stock = (stock != value) ? value : stock;
        }

        public TreeViewUserControl()
        {
            InitializeComponent();
            Grid.DataContext = this;
            var dis1 =
       Finance.Sectors
         .ToObservable(Scheduler.Default)
         .ToObservableChangeSet()
         .ObserveOnDispatcher()
            .DisposeMany()
         .Bind(out sectors)
         .Subscribe(_ =>
         Stock = sectors.First().Stocks.First());
        }
    }
}