using Jellyfish;
using MintPlayer.ObservableCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Utility.Extensions;
using Utility.Helpers.Ex;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Reactives;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomStyleUserControl : UserControl
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


        public CustomStyleUserControl()
        {
            InitializeComponent();
            this.Loaded += CustomStyleUserControl_Loaded;

        }

        private void CustomStyleUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
        }

        void Initialise()
        {
            var _collection = TreeViewHelper.VisibleItems(MyTreeView).ToCollection(out _);      
            var foo = new Uri("/Utility.WPF.Controls.Trees;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            var resourceDictionary = new ResourceDictionary() { Source = foo };
            var collection = new ObservableCollection<ButtonViewModel>();
            ItemsControl.ItemsSource = collection;

            try
            {
                foreach (var style in FindResourcesByType(resourceDictionary, typeof(TreeViewItem)).ToArray())
                {

                    collection.Add(new ButtonViewModel
                    {
                        Header = style.Key,
                        Command = new RelayCommand((a) =>
                        {
                            MyTreeView.ItemContainerStyleSelector = null;
                            TreeItemContainerStyleSelector.Instance.Current = style.Value;
                            MyTreeView.ItemContainerStyleSelector = TreeItemContainerStyleSelector.Instance;

                            foreach (var item in _collection.ToArray())
                            {
                                item.ItemContainerStyleSelector = null;
                                item.ItemContainerStyleSelector = TreeItemContainerStyleSelector.Instance;
                            }
                        })
                    });

                }
            }
            catch (Exception ex) { }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyTreeView.ItemContainerStyleSelector = null;
            MyTreeView.ItemContainerStyleSelector = TreeItemContainerStyleSelector.Instance;
        }


        private IEnumerable<KeyValuePair<string?, Style>> FindResourcesByType(ResourceDictionary resources, Type type)
        {
            return resources.MergedDictionaries.SelectMany(d => FindResourcesByType(d, type)).Union(resources
                .Cast<DictionaryEntry>()
                .Where(s => s.Value is Style style && style.TargetType == type)
                .Select(a => new KeyValuePair<string?, Style>(a.Key?.ToString(), (Style)a.Value)));
        }



    }



    public class TreeItemContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {

            return Current;

            //return base.SelectStyle(item, container);
        }

        public Style Current { get; set; }

        public static TreeItemContainerStyleSelector Instance { get; } = new();
    }

}
