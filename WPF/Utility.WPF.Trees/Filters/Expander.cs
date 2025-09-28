using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;

namespace Utility.Nodes.WPF
{
    public class Expander : IExpander
    {
        public Expander()
        {
            Predicate =
                new FalseDecisionTree<IReadOnlyTree>(new Decision(item => true))
                {
                    // prevents child items of a parent Descriptor that is being shown as a Treeviewer from also being shown
                    new FalseDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item) as IDescriptor != null), a=> true)
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
