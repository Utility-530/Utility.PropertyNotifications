using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Decisions;

namespace Utility.Trees.Demo.MVVM.Infrastructure
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
            return backputs.FirstOrDefault(a => a is DataTemplate);
        }
    }


}
