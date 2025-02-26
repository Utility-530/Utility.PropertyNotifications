using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Repos;
using ReactiveUI;
using System.Linq;
using Utility.Extensions;
using System.Reactive.Linq;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.Helpers.Ex;
using Utility.Structs.Repos;
using Utility.Trees.Extensions.Async;

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
                .WhereNotNull()
                .Subscribe(a =>
                {

                    var name = a.Name;
                    var keys = TreeRepository.Instance.SelectKeys(table_name: name).GetAwaiter().GetResult();


                    var tree = keys.ToObservable().ToTree(a => a.Guid, a => a.ParentGuid, a.Guid, func: a =>
                    {
                        var _tree = (ITree<Key>)new Tree<Key>(a);
                        var get = TreeRepository.Instance.Get(a.Guid).Subscribe(a =>
                        {
                            if (a.Value is { }  x)
                                _tree.Add(new Tree<Key>(new Key(default, default, x.GetType(), default, default, default)));
                        });

                        return _tree;
                    });
                    TreeView.ItemsSource = tree.ToObservableCollection();
                });

        }
    }
}
