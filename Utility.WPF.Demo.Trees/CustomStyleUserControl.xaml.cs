using Jellyfish;
using MintPlayer.ObservableCollection;
using NetFabric.Hyperlinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomStyleUserControl : UserControl
    {
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
            List<TreeViewItem> list = new();
            TreeEnumerator.GetTreeViewItems(MyTreeView).ToArray();
            var foo = new Uri("/Utility.WPF.Controls.Trees;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            var resourceDictionary = new ResourceDictionary() { Source = foo };
            var collection = new ObservableCollection<ButtonViewModel>();
            ItemsControl.ItemsSource = collection;

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

                        foreach (var item in list)
                        {
                            item.ItemContainerStyleSelector = null;
                            item.ItemContainerStyleSelector = TreeItemContainerStyleSelector.Instance;
                        }
                    })
                });

            }

            TreeEnumerator.Observable.Subscribe(a =>
            {
                if (a.Action == NotifyCollectionChangedAction.Add)
                {
                    list.Add(a.NewItems.Cast<TreeViewItem>().Single());
                }
                if (a.Action == NotifyCollectionChangedAction.Remove)
                {
                    list.Remove(a.OldItems.Cast<TreeViewItem>().Single());
                }
            });
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
