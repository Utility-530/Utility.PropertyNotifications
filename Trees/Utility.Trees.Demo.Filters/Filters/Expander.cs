using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;

namespace Utility.Trees.Demo.Filters.Filters
{
    public class Expander : IExpander
    {
        public Expander()
        {
            Predicate =
                new FalseDecisionTree<IReadOnlyTree>(new Decision(item => true))
                {
                    // prevents child items of a parent Descriptor that is being shown as a Treeviewer from also being shown
                    new FalseDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item).Data as IDescriptor != null), a=> true)
                    {
                    }
                };
        }

        public DecisionTree Predicate { get; set; }


        public bool Expand(object item)
        {
            Predicate.Reset();
            Predicate.Input = item;
            Predicate.Evaluate();
            return (bool)Predicate.Backput;
        }

        public static Expander Instance { get; } = new();
    }
}
