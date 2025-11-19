using System.Windows;
using System.Windows.Controls;
using Utility;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Trees.Decisions;
using Utility.WPF.Trees;

namespace Utility.WPF.Trees
{
    public class TreeViewItemDecisionTree : DecisionTree<object, ItemsControl>
    {
        public TreeViewItemDecisionTree(IDecision decision, Func<object, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }
    }

    public class DataTemplateDecisionTree<T> : DecisionTree<T, DataTemplate>
    {
        public DataTemplateDecisionTree(IDecision decision, Func<T, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }
    }

    public class DataTemplateDecisionTree : DecisionTree
    {
        public DataTemplateDecisionTree(IDecision decision, Func<object, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }

        protected override object ToBackPut(ICollection<object> backputs)
        {
            return (backputs.FirstOrDefault(a => a is Func<DataTemplate>) as Func<DataTemplate>)?.Invoke();
        }
    }
}