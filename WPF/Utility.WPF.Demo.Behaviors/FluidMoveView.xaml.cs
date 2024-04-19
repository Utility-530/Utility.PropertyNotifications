using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;


namespace Utility.WPF.Demo.Behaviors
{
    /// <summary>
    /// Interaction logic for FluidMoveView.xaml
    /// </summary>
    public partial class FluidMoveView : UserControl
    {

        DispatcherTimer _timer = new DispatcherTimer();
        List<StockItem> _items = new List<StockItem>();
        ObservableCollection<StockItem> _filteredItems = new ObservableCollection<StockItem>();
        ICollectionViewLiveShaping view;
        public FluidMoveView()
        {
            InitializeComponent();

            var rnd = new Random();
            string[] names = { "ABCD", "QTYD", "DHJD", "OIUI", "TRET", "AGSD", "YTJD", "MHHJ", "FRGD", "HDHE", "JAGH", "PERI" };
            Array.ForEach(names, s => _items.Add(new StockItem { Name = s }));


            view = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(_items);
            view.IsLiveSorting = true;
            view.IsLiveFiltering = true;
            view.LiveFilteringProperties.Add(nameof(StockItem.Value));
            view.LiveSortingProperties.Add(nameof(StockItem.Value));
            _grid.ItemsSource = (IEnumerable)view;

            _timer.Interval = TimeSpan.FromSeconds(3);

            foreach (var item in _items)
            {
                item.Value += (int)(rnd.NextDouble() * rnd.Next(-1, 2) * 10);
            }

            _timer.Tick += (s, e) =>
            {
                using (((ICollectionView)view).DeferRefresh())
                {
                    foreach (var item in _items)
                    {
                        item.Value += (int)(rnd.NextDouble() * rnd.Next(-1, 2) * 10);
                    }                
                }
                Sort();
            };

            _grid.ItemContainerGenerator.StatusChanged += (sender, e) =>
            {
                Sort();
            };

            void Sort()
            {
                var ordered = _items.OrderByDescending(a => a.Value).Take(5).ToArray();
                foreach (var item in _items)
                {
                    var order = ordered.Select((a, i) => (a, i)).FirstOrDefault(a => a.a.Value.Equals(item.Value)).i;

                    //_grid.ScrollIntoView(_grid.Items[order]);
                    if (_grid.ItemContainerGenerator.ContainerFromItem(item) is UIElement element)
                        Grid.SetRow(element, order + 1);
                    else
                    {
                    }
                }

            }

            _timer.Start();
            //((ICollectionView)view).SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Descending));
            ((ListCollectionView)view).CustomSort = new CustomerSorter();

            ((ICollectionView)view).Filter += e =>
            {
                var ordered = _items.OrderByDescending(a => a.Value).ToArray();
                var last = ordered.Take(5).Last().Value;
                if (e is StockItem { Value: int value })
                {
                    return value >= last;
                }
                return false;
            };
        }
    }

    public class CustomerSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            StockItem custX = x as StockItem;
            StockItem custY = y as StockItem;
            return custY.Value.CompareTo(custX.Value);
        }
    }

    class StockItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        Stats stats = new();
        int _value;
        private Task<Stat> pointsTask;

        public string Name { get; set; }


        public int? HolesPlayed
        {
            get
            {
                if (pointsTask?.Status == TaskStatus.RanToCompletion)
                {
                    return pointsTask.Result.Value;
                }
                else
                    pointsTask = Task.Run(async () =>
                    {
                        var stat = await stats.FindOrCreateAsync("Points").ConfigureAwait(false);
                        //stat.PropertyChanged += (s, e) =>
                        //{
                        //    raisePropertyChanged(nameof(Points));
                        //};

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HolesPlayed"));
                        return stat;
                    });
                return null;
            }
        }



        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

    }


    public class Stats
    {
        Random random = new();
        public async Task<Stat> FindOrCreateAsync(string v)
        {
            await Task.Delay(random.Next(0, 10000));
            return new Stat();
        }
    }

    public class Stat
    {
        public int Value { get; } = 10;
    }
}

