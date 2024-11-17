using Jellyfish;
using NetFabric.Hyperlinq;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Enums;
using Utility.Helpers;
using Utility.Trees.Abstractions;
using Utility.Trees.Demo.Infrastructure;
using Svc = Utility.Trees.Demo.Infrastructure.Service;

namespace Utility.Trees.Demo
{
    /// <summary>
    /// Interaction logic for TreeView.xaml
    /// </summary>
    public partial class DynamicTreeView : UserControl
    {
        DynamicTree tree = new(new Tree(new Persist() { Guid = Guid.NewGuid(), Name = "root" }));

        public DynamicTreeView()
        {
            InitializeComponent();

            DirectionButtons.Command = new RelayCommand<Direction>((direction) =>
            {
                switch (direction)
                {
                    case Direction.Left:
                        tree.State = State.Back;
                        break;
                    case Direction.Right:
                        tree.State = State.Forward;
                        tree.State = State.Forward;
                        break;
                    case Direction.Up:
                        tree.State = State.Up;
                        break;
                    case Direction.Down:
                        tree.State = State.Down;
                        break;
                }
            });

            EditCollectionButtons.Command = new RelayCommand<AddRemove>((addRemove) =>
            {
                switch (addRemove)
                {
                    case AddRemove.Add:
                        tree.State = State.Add;
                        break;
                    case AddRemove.Remove:
                        tree.State = State.Remove;
                        break;
                }
            });

            PersistenceButtons.Command = new RelayCommand<Persistence>((loadSave) =>
            {
                switch (loadSave)
                {
                    case Persistence.Load:
                        var load = TreePersist.Instance.Load<Persist>();
                        if (load.Data != null)
                            tree.Tree = (load);
                        break;
                    case Persistence.Save:
                        TreePersist.Instance.Save(tree.Tree);
                        break;
                }
            });

            //NewButton.Command = new RelayCommand((a) =>
            //{
            //    tree.Tree = new PersistTree(new Persist() { Name = "root" });
            //});

            var bootStrapper = new Bootstrapper();

            ConnectionsView.TreeContent.DataContext = tree;
            DirectionButtons.Enabled = Direction.None;
            EditCollectionButtons.Enabled = AddRemove.None;
            ConnectionsView.ViewModel = new ConnectionsViewModel()
            {
                ServiceModel = bootStrapper.ResolveMany<Svc>().ToList(),
                ViewModel = tree.Tree
            };

            _ = tree.WhenAnyValue(a => a.State)
                .Subscribe(Change);
        }


        private void Change(State state)
        {
            Direction direction = Direction.None;
            AddRemove addRemove = AddRemove.None;

            foreach (var flag in EnumHelper.SeparateFlags(state))
            {
                if (flag == State.Add)
                {
                    addRemove |= AddRemove.Add;
                }
                if (flag == State.Remove)
                {
                    addRemove |= AddRemove.Remove;

                }
                if (flag == State.Back)
                {
                    direction |= Direction.Left;
                }
                if (flag == State.Forward)
                {
                    direction |= Direction.Right;

                }
                if (flag == State.Up)
                {
                    direction |= Direction.Up;

                }
                if (flag == State.Down)
                {
                    direction |= Direction.Down;

                }
            }
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Sex");
            DirectionButtons.Enabled = direction;
            EditCollectionButtons.Enabled = addRemove;
        }

        private void Generate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeView.Children.Clear();

            var _tree = tree.Tree;
            var _treeView = new TreeView();
            TreeHelper.ExploreTree(
                _treeView.Items,
                (a, b) => { a.Add(new TreeViewItem() { Header = (b.Data), HeaderTemplateSelector = HeaderTemplateSelector }); return a; },
                (a, b) => { },
                _tree,
                (a) =>
                {
                    return System.Reactive.Linq.Observable.Create<NotifyCollectionChangedEventArgs>(observer =>
                    {
                        observer.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)a.Items));
                        return Disposable.Empty;
                    });
                },
                SelectNewItems<ITree>,
                SelectOldItems<ITree>);

            TreeView.Children.Add(_treeView);

            static IEnumerable<T> SelectNewItems<T>(NotifyCollectionChangedEventArgs args)
            {
                return args.NewItems?.Cast<T>() ?? Array.Empty<T>();
            }

            static IEnumerable<T> SelectOldItems<T>(NotifyCollectionChangedEventArgs args)
            {
                return args.OldItems?.Cast<T>() ?? Array.Empty<T>();
            }
        }

        HeaderTemplateSelector HeaderTemplateSelector { get; } = new();
    }

    public class HeaderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var buttonTemplate = Application.Current.Resources["Button"] as DataTemplate;
            if (buttonTemplate != null)
            {
                return buttonTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
