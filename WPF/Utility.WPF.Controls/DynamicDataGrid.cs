using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using CustomHelper;

namespace Utility.WPF.Controls
{
    // A datagrid which acepts a dictionary as its itemssource by converting keyvaluepair to custom dynamic class
    public class DynamicDataGrid : DataGrid
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(DynamicDataGrid), new PropertyMetadata("Key", KeyChange));

        //public static readonly DependencyProperty ItemsSinkProperty =    DependencyProperty.Register("ItemsSink", typeof(IEnumerable), typeof(DynamicDataGrid), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(DynamicDataGrid), new PropertyMetadata("Value", ValueChange));

        //public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IEnumerable), typeof(DynamicDataGrid), new PropertyMetadata(null, DataChanged));

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        //public IEnumerable Data
        //{
        //    get { return (IEnumerable)GetValue(DataProperty); }
        //    set { SetValue(DataProperty, value); }
        //}

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (!(((IEnumerable)e.NewValue).First() is Dynamic))
            //    (d as DynamicDataGrid).ChangeItemsSource((IEnumerable)e.NewValue);
        }

        protected virtual void ChangeItemsSource(IEnumerable value)
        {
            //ItemsSourceSubject.OnNext((IEnumerable)value);
        }

        private static void ValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DynamicDataGrid).ValueChangeSubject.OnNext((string)e.NewValue);
        }

        private static void KeyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DynamicDataGrid).KeyChangeSubject.OnNext((string)e.NewValue);
        }

        //private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as DynamicDataGrid).DataChangeSubject.OnNext((IEnumerable)e.NewValue);
        //}

        protected ISubject<IEnumerable> ItemsSourceSubject = new Subject<IEnumerable>();
        protected ISubject<string> KeyChangeSubject = new Subject<string>();
        protected ISubject<string> ValueChangeSubject = new Subject<string>();

        static DynamicDataGrid()
        {
            ItemsSourceProperty.OverrideMetadata(typeof(DynamicDataGrid), new FrameworkPropertyMetadata(null, ItemsSourceChanged));
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicDataGrid), new FrameworkPropertyMetadata(typeof(DynamicDataGrid)));
        }

        public DynamicDataGrid()
        {
            var obs = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => this.Loaded += h, h => this.Loaded -= h).Select(_ => 0);
            var obs2 = Observable.When(KeyChangeSubject.And(ValueChangeSubject).And(ItemsSourceSubject).Then((a, b, c) => 0));

            obs.Amb(obs2)
                .CombineLatest(KeyChangeSubject.StartWith(Key).DistinctUntilChanged(), (a, b) => b)
                .CombineLatest(ValueChangeSubject.StartWith(Value).DistinctUntilChanged(), (a, b) => b)
                .CombineLatest(ItemsSourceSubject.DistinctUntilChanged(), (a, b) => b)
                .Subscribe(_ =>
                {
                    this.Dispatcher.InvokeAsync(() => this.SetValue(ItemsSourceProperty, DynmamicHelper.OnGetData(_, Key, Value)), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
                });
        }
    }
}