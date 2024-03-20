using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Descriptors.Repositorys;
using ReactiveUI;
using System.Linq;
using Utility.Extensions;
using System.Reactive.Linq;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Database
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SQLitePCL.Batteries.Init();
            var rootKeys = TreeRepository.Instance.SelectKeys().GetAwaiter().GetResult();

            ComboBox.ItemsSource = rootKeys;
            ComboBox.DisplayMemberPath = nameof(Key.Name);
            ComboBox.SelectedIndex = 0;
            ComboBox.Events().SelectionChanged
                .SelectMany(a => a.AddedItems.Cast<Key>())
                .StartWith((Key)ComboBox.SelectedItem)
                .WhereNotNull()
                .Subscribe(a =>
                {

                    var name = a.Name;
                    var keys = TreeRepository.Instance.SelectKeys(table_name: name).GetAwaiter().GetResult();


                    var tree = TreeExtensions.ToTree(keys, a => a.Guid, a => a.ParentGuid, a =>
                    {
                        var tree = (ITree<Key>)new Tree<Key>(a);
                        var get = TreeRepository.Instance.Get(a.Guid);
                        if (get != null)
                            tree.Add(new Tree<Key>(new Key(default, default, default, get.Value.ToString(), default)));
                        return tree;
                    }
                    , a.Guid).ToArray();
                    TreeView.ItemsSource = tree;
                });

        }
    }
}
