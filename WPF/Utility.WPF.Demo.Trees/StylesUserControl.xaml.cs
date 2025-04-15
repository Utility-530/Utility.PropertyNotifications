using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Utility.Collections;
using Utility.Commands;
using Utility.Extensions;
using Utility.Helpers.Ex;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.WPF.Controls.Trees;
using Utility.WPF.Demo.Common.ViewModels;
using Arrangement = Utility.Enums.Arrangement;
using Orientation = Utility.Enums.Orientation;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StylesUserControl : UserControl
    {

        private static void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                subject.OnNext(new(NotifyCollectionChangedAction.Remove, treeViewItem));
            }
        }

        private static void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                subject.OnNext(new(NotifyCollectionChangedAction.Add, treeViewItem));
            }
        }
        static ReplaySubject<NotifyCollectionChangedEventArgs> subject = new ReplaySubject<NotifyCollectionChangedEventArgs>();

        public static IObservable<NotifyCollectionChangedEventArgs> Observable => subject;

        public SettingsViewModel Settings { get; } = new();

        public IEnumerable Items { get; set; }

        public class TreeCollection : Collection
        {
            public TreeCollection()
            {
                Comparer = new Comparer();
                this.CollectionChanged += TreeCollection_CollectionChanged;
            }

            private void TreeCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                foreach(var x in e.NewItems)
                {
                    if(x is _Node node)
                    {
                        node.WithChangesTo(a => a.Position).Subscribe(a =>
                        {
                            Sort();
                        });
                    }
                }
            }
        }

        public StylesUserControl()
        {
            InitializeComponent();
            this.Loaded += CustomStyleUserControl_Loaded;

            Items = new[] { new _Node { Items = new TreeCollection { new _Node(), new _Node() { Items = new TreeCollection { new _Node(), new _Node() } }, new _Node { Items = new TreeCollection { new _Node() } } } } };

        }

        public class Comparer : IComparer<object>
        {
            public int Compare(object? x, object? y)
            {
                return (x as _Node)?.Position.CompareTo((y as _Node)?.Position) ?? 0;
            }

        }
        private void CustomStyleUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
        }

        void Initialise()
        {
            var _collection = Reactives.TreeViewHelper.VisibleItems(MyTreeView).ToCollection(out _);
            var foo = new Uri("/Utility.WPF.Controls.Trees;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            var resourceDictionary = new ResourceDictionary() { Source = foo };
            var collection = new ObservableCollection<ButtonViewModel>();


            try
            {
                foreach (var style in FindResourcesByType(resourceDictionary, typeof(CustomTreeViewItem)).ToArray())
                {

                    collection.Add(new ButtonViewModel
                    {
                        Header = style.Key,
                        Command = new Command(() =>
                        {
                            MyTreeView.ItemContainerStyleSelector = null;
                            CustomTreeItemContainerStyleSelector.Instance.Current = style.Value;
                            MyTreeView.ItemContainerStyleSelector = CustomTreeItemContainerStyleSelector.Instance;

                            foreach (var item in _collection.ToArray())
                            {
                                item.ItemContainerStyleSelector = null;
                                item.ItemContainerStyleSelector = CustomTreeItemContainerStyleSelector.Instance;
                            }
                        })
                    });
                }

                Settings.Buttons = collection;
                Settings.RaisePropertyChanged(nameof(Settings.Buttons));
            }


            catch (Exception ex) { }


        }


        private IEnumerable<KeyValuePair<string?, Style>> FindResourcesByType(ResourceDictionary resources, Type type)
        {
            return resources.MergedDictionaries.SelectMany(d => FindResourcesByType(d, type)).Union(resources
                .Cast<DictionaryEntry>()
                .Where(s => s.Value is Style style && style.TargetType == type)
                .Select(a => new KeyValuePair<string?, Style>(a.Key?.ToString(), (Style)a.Value)));
        }
    }

    public class _Node : NotifyPropertyClass
    {
        private bool isEditable = true;

        static Random random { get; } = new();

        public _Node()
        {
            DispatcherTimer d = new() { Interval = new TimeSpan(0,0,5) };
            //d.Start();
            d.Tick += D_Tick;
        }

        private void D_Tick(object? sender, EventArgs e)
        {
            Position = Utility.Helpers.RandomHelper.NextInteger(random, 0, 3);
            RaisePropertyChanged(nameof(Position));
        }

        public bool IsEditable
        {
            get => isEditable; set
            {
                isEditable = value;
                RaisePropertyChanged(nameof(IsEditable));
            }
        }
        public int Position { get; set; } = Utility.Helpers.RandomHelper.NextInteger(random, 0, 3);

        public IEnumerable Items { get; set; }

    }


    public class SettingsViewModel : NotifyPropertyClass
    {
        private int rows;
        private int columns;
        private Orientation orientation;
        private Arrangement arrangement;

        public int Rows { get => rows; set => this.RaisePropertyChanged(ref rows, value); }
        public int Columns { get => columns; set => this.RaisePropertyChanged(ref columns, value); }
        public Orientation Orientation { get => orientation; set => this.RaisePropertyChanged(ref orientation, value); }
        public Arrangement Arrangement { get => arrangement; set => this.RaisePropertyChanged(ref arrangement, value); }

        public IEnumerable Buttons { get; set; }
    }

    public class CustomTreeItemContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {

            return Current;

            //return base.SelectStyle(item, container);
        }

        public Style Current { get; set; }

        public static CustomTreeItemContainerStyleSelector Instance { get; } = new();
    }

}
