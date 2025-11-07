using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using IIndex = Utility.Interfaces.Trees.IIndex;
using Size = System.Windows.Size;

namespace Utility.WPF.Panels
{
    public class TreeNode : ITreeIndex, IComparable
    {
        public TreeNode(IIndex index)
        {
            Index = index;
        }

        public IIndex Index { get; }
        public UIElement Element { get; set; }
        public List<TreeNode> Children { get; set; } = [];
        public int Level => Index?.Count ?? 0;

        public int CompareTo(object? obj)
        {
            if (obj is ITreeIndex { Index: { } index })
                return Index.CompareTo(index);
            throw new Exception("224ik e3s");
        }
    }

    // Custom Panel that arranges items in a hierarchical tree structure
    public class TreePanel : Panel
    {
        public static readonly DependencyProperty IndentSizeProperty = DependencyProperty.Register(nameof(IndentSize), typeof(double), typeof(TreePanel), new PropertyMetadata(20.0, OnLayoutPropertyChanged));
        public static readonly DependencyProperty ItemSpacingProperty = DependencyProperty.Register(nameof(ItemSpacing), typeof(double), typeof(TreePanel), new PropertyMetadata(5.0, OnLayoutPropertyChanged));
        public static readonly DependencyProperty KeyPropertyNameProperty = DependencyProperty.Register("KeyPropertyName", typeof(string), typeof(TreePanel), new PropertyMetadata("Key"));

        public string KeyPropertyName
        {
            get { return (string)GetValue(KeyPropertyNameProperty); }
            set { SetValue(KeyPropertyNameProperty, value); }
        }

        public double IndentSize
        {
            get => (double)GetValue(IndentSizeProperty);
            set => SetValue(IndentSizeProperty, value);
        }

        public double ItemSpacing
        {
            get => (double)GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreePanel panel)
            {
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var treeNodes = BuildTreeStructure(Children, KeyPropertyName);
            double maxWidth = 0;
            double totalHeight = 0;

            MeasureTreeNodes(treeNodes, availableSize, IndentSize, ItemSpacing, ref maxWidth, ref totalHeight);
            return new Size(maxWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var treeNodes = BuildTreeStructure(Children, KeyPropertyName);
            double currentY = 0;
            ArrangeTreeNodes(treeNodes, 0, IndentSize, ItemSpacing, ref currentY);
            return finalSize;
        }

        public static List<TreeNode> BuildTreeStructure(UIElementCollection Children, string? KeyPropertyName = null)
        {
            var allNodes = new List<TreeNode>();

            foreach (UIElement child in Children)
            {
                var treeItem = GetTreeItem(child, KeyPropertyName);
                if (treeItem != null)
                {
                    var node = new TreeNode(treeItem.Index)
                    {
                        Element = child,
                    };
                    allNodes.Add(node);
                }
            }

            allNodes.Sort();

            var rootNodes = new List<TreeNode>();
            foreach (var node in allNodes)
            {
                if (node.Level == 1)
                {
                    rootNodes.Add(node);
                }
                else if (node.Level > 1)
                {
                    var parent = FindParentNode(allNodes, node.Index);
                    parent?.Children.Add(node);
                }
            }

            return rootNodes;
        }

        private static ITreeIndex GetTreeItem(UIElement element, string? KeyPropertyName = null)
        {
            if (element is ITreeIndex directItem)
                return directItem;
            else if (element is FrameworkElement fe && fe.DataContext is ITreeIndex contextItem)
                return contextItem;
            else if (element.GetType().GetProperties().FirstOrDefault(p => p.Name == KeyPropertyName) is { } property)
            {
                var key = property.GetValue(element) as string;
                if (key != null)
                {
                    return new TreeNode((Utility.Structs.Index)key) { Element = element };
                }
            }
            else if (element is FrameworkElement _fe && _fe.DataContext.GetType().GetProperties().FirstOrDefault(p => p.Name == KeyPropertyName) is { } _property)
            {
                var key = _property.GetValue(_fe.DataContext) as string;
                if (key != null)
                {
                    return new TreeNode((Utility.Structs.Index)key) { Element = element };
                }
            }
            throw new Exception($"Element of type {element.GetType()} does not implement ITreeIndex or have a valid DataContext with ITreeIndex or KeyPropertyName '{KeyPropertyName}'.");
        }

        public static TreeNode FindParentNode(List<TreeNode> allNodes, IReadOnlyList<int> childPath)
        {
            if (childPath == null || childPath.Count <= 1)
                return null;

            var parentPath = childPath.Take(childPath.Count - 1).ToArray();
            return allNodes.FirstOrDefault(node =>
                node.Index != null &&
                node.Index.Count == parentPath.Length &&
                node.Index.Equals((Utility.Structs.Index)parentPath));
        }

        public static void MeasureTreeNodes(List<TreeNode> nodes, Size availableSize, double IndentSize, double ItemSpacing, ref double maxWidth, ref double totalHeight)
        {
            foreach (var node in nodes)
            {
                node.Element.Measure(availableSize);
                var elementSize = node.Element.DesiredSize;
                double nodeWidth = (node.Level - 1) * IndentSize + elementSize.Width;
                maxWidth = Math.Max(maxWidth, nodeWidth);
                totalHeight += elementSize.Height + ItemSpacing;

                if (node.Children.Count > 0)
                {
                    MeasureTreeNodes(node.Children, availableSize, IndentSize, ItemSpacing, ref maxWidth, ref totalHeight);
                }
            }
        }

        public static void ArrangeTreeNodes(List<TreeNode> nodes, int level, double IndentSize, double ItemSpacing, ref double currentY)
        {
            foreach (var node in nodes)
            {
                var elementSize = node.Element.DesiredSize;
                double x = level * IndentSize;
                node.Element.Arrange(new Rect(x, currentY, elementSize.Width, elementSize.Height));
                if (node.Element is ILocation location)
                {
                    location.Location = new PointF((float)(x + elementSize.Width / 2), (float)(currentY + elementSize.Height / 2));
                }
                currentY += elementSize.Height + ItemSpacing;

                if (node.Children.Count > 0)
                {
                    ArrangeTreeNodes(node.Children, level + 1, IndentSize, ItemSpacing, ref currentY);
                }
            }
        }
    }
}