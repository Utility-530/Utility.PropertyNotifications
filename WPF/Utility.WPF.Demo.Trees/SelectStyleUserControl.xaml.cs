using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using MintPlayer.ObservableCollection;
using Utility.Commands;
using Utility.Extensions;
using Utility.Helpers.Ex;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Reactives;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SelectStyleUserControl : UserControl
    {
        public SelectStyleUserControl()
        {
            InitializeComponent();
            this.Loaded += CustomStyleUserControl_Loaded;

            void CustomStyleUserControl_Loaded(object sender, RoutedEventArgs e)
            {
                var collection = new ObservableCollection<ButtonViewModel>();
                ItemsControl.ItemsSource = collection;
                foreach (var item in Initialise())
                {
                    collection.Add(item);
                }
                IEnumerable<ButtonViewModel> Initialise()
                {
                    var _collection = TreeViewHelper.VisibleItems(MyTreeView).ToCollection(out _);
                    var foo = new Uri("/Utility.WPF.Controls.Trees;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                    var resourceDictionary = new ResourceDictionary() { Source = foo };

                    foreach (var style in FindResourcesByType(resourceDictionary, typeof(TreeViewItem)).ToArray())
                    {
                        yield return new ButtonViewModel
                        {
                            Header = style.Key,
                            Command = new Command(() =>
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
                        };
                    }
                    IEnumerable<KeyValuePair<string?, Style>> FindResourcesByType(ResourceDictionary resources, Type type)
                    {
                        return resources.MergedDictionaries.SelectMany(d => FindResourcesByType(d, type)).Union(resources
                            .Cast<DictionaryEntry>()
                            .Where(s => s.Value is Style style && style.TargetType == type)
                            .Select(a => new KeyValuePair<string?, Style>(a.Key?.ToString(), (Style)a.Value)));
                    }
                }
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