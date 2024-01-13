using Utility.Infrastructure;
using Utility.Nodes;
using Utility.Observables.Generic;
using Utility.Trees.Abstractions;
using Utility.Models;
using System.Windows.Controls;
using System.Windows;
using System;
using Utility.PropertyTrees.WPF.Meta;
using Utility.WPF.Reactive;

namespace Utility.PropertyTrees.Demo.ViewModels
{
    public class ViewController : BaseObject
    {
        private TreeView treeView;
        private RootProperty node;
        private Grid grid;
        private TreeView dataGrid = new();
        private GetViewModelResponse response;
        private ValueNode valueNode;

        public override Key Key => new(Guids.ViewController, nameof(ViewController), typeof(ViewController));

        public void OnNext(StartEvent startEvent)
        {
            Context.Post((_) =>
            {
                var content = CreateContent(node = startEvent.Property);
                var window = new Window { Content = content };
                window.Show();
            }, default);

            object CreateContent(ValueNode valueNode)
            {
                treeView = new();
                CreateSelected(valueNode);

                grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Grid.SetRow(treeView, 0);
                grid.Children.Add(treeView);

#if DEBUG
                //CreateDebugContent();
#endif
                return grid;
            }
        }

        private void CreateSelected(ValueNode valueNode)
        {
            this.valueNode = valueNode;
            Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
                .Subscribe(a =>
                {
                });

            treeView
                .MouseMoves()
                .Subscribe(a =>
                {
                    if (a.item is { Header: ITree { } node })
                    {
                        Structs.Point sPoint = new(a.point.X, a.point.Y);
                        Send(new OnHoverChange(a.item, node, true, sPoint));
                    }
                });

            treeView
                .MouseHoverLeaves()
                .Subscribe(a =>
                {
                    if (a is { Header: ITree { } node })
                    {
                        Send(new OnHoverChange(a, node, false, default));
                    }
                });
        }

        public void OnNext(RefreshRequest request)
        {
            Context.Post((_) =>
            {
                treeView.Items.Clear();
                Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
                .Subscribe();

            }, null);
        }
    }
}