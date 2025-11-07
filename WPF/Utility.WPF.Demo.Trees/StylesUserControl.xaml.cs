using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Utility.Commands;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Extensions;
using Utility.Helpers.Ex;
using Utility.Nodes;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.WPF.Controls.Trees;
using Utility.WPF.Demo.Common.ViewModels;
using NodeVM = Utility.Nodes.NodeViewModel;

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

        private static ReplaySubject<NotifyCollectionChangedEventArgs> subject = new ReplaySubject<NotifyCollectionChangedEventArgs>();

        public static IObservable<NotifyCollectionChangedEventArgs> Observable => subject;

        public SettingsViewModel Settings { get; } = new();

        public IEnumerable Items { get; set; }

        public StylesUserControl()
        {
            InitializeComponent();
            this.Loaded += CustomStyleUserControl_Loaded;

            Items = new[] { NodeVM.Create(null, [NodeVM.Create(null, []), NodeVM.Create(null, [NodeVM.Create(null, []), NodeVM.Create(null, [])]), NodeVM.Create(null, [NodeVM.Create(null, [])])]) };

            JsonConvert.DefaultSettings = () => settings;
        }

        private JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            //Formatting = Formatting.Indented,
            Converters = [

                new StringEnumConverter(),
                //new TypeConverter(),

                new NodeConverter(),
                new MetadataConverter()
                    ]
        };

        public class Comparer : IComparer<object>
        {
            public int Compare(object? x, object? y)
            {
                return (x as NodeVM)?.Order.CompareTo((y as NodeVM)?.Order) ?? 0;
            }
        }

        private void CustomStyleUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
        }

        private void Initialise()
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

    public class SettingsViewModel : NotifyPropertyClass
    {
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