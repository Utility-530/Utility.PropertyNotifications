using System.Linq.Expressions;
using System.Text;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Pipes;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Decisions
{
    public interface IDecisionTreeX : ITree
    {
        bool IsInvoked { get; set; }

        object Input { get; set; }
        bool IsBackputSet { get; }
        object Backput { get; }

        void Evaluate(List<object> keys);

        void BackPropagate(List<object> keys);
    }

    public class DecisionTreeX<T> : DecisionTree<T>, IDecisionTreeX
    {
        private bool isInvoked;

        public bool IsInvoked
        {
            get => isInvoked;
            set
            {
                isInvoked = value;
                RaisePropertyChanged();
            }
        }


        public bool IsBackputSet { get; set; }

        private readonly Func<IEnumerable<object>, object?> combine;

        public DecisionTreeX(Expression<Func<T, bool>> decision, string key, Func<T, object>? transform = null, Func<IEnumerable<object>, object?>? combine = null) : base(new Decision<T>(decision), transform)
        {
            Key = key;
            this.combine = combine ?? new Func<IEnumerable<object>, object?>(a =>
        {
            return a.FirstOrDefault(v => v != null);
        });
        }

        protected override object ToBackPut(ICollection<object> backputs)
        {
            return combine.Invoke(backputs);
        }

        public virtual void Evaluate(List<object> keys)
        {
            var result = decision.Evaluate(Input);
            IsBackputSet = false;
            if (result == Match)
            {
                Output = Transform(Input);
                if (Items.Any())
                {
                    foreach (IDecisionTreeX item in Items)
                    {
                        (item as ISetParent<IDecisionTreeX>).Parent = this;
                        var queueItem = new ForwardItem(item, Output, new List<object>(keys));
                        Pipe.Instance.Queue(queueItem);
                    }
                    return;
                }
                else if (this.Parent is IDecisionTreeX decisionTreeX)
                {
                    Backput = this.Output;
                    IsBackputSet = true;
                    Pipe.Instance.GoBack(new BackItem2(decisionTreeX, Backput, new List<object>(keys)));
                }
            }
            else
            {
                Backput = Output = null;
                IsBackputSet = true;
            }
        }


        public virtual void BackPropagate(List<object> keys)
        {
            List<object> outputs = new();

            foreach (IDecisionTreeX item in Items)
            {
                if (item.IsBackputSet == false)
                    return;
                outputs.Add(item.Backput);
            }

            Backput = ToBackPut(outputs);
            IsBackputSet = true;
            if (this.Parent is IDecisionTreeX x)
            {
                var queueItem = new BackItem(x, Backput, new List<object>(keys));
                Pipe.Instance.GoBack(queueItem);
            }
        }

    }

    public record ForwardItem(IDecisionTreeX Tree, object Value, List<object> Keys) : DecisionQueueItem(Tree, Value, Keys)
    {
        public override void Invoke()
        {
            Decider.Instance.Current = Tree;
            Keys.Add(this);
            Tree.Input = Value;
            Tree.Evaluate(Keys);
        }
    }


    public record BackItem2(IDecisionTreeX Tree, object Value, List<object> Keys) : BackItem(Tree, Value, Keys)
    {
    }

    public record BackItem(IDecisionTreeX Tree, object Value, List<object> Keys) : DecisionQueueItem(Tree, Value, Keys)
    {
        public override void Invoke()
        {
            Decider.Instance.Current = Tree;
            Keys.Add(this);
            Tree.BackPropagate(Keys);
        }
    }

    public abstract record DecisionQueueItem(IDecisionTreeX Tree, object Value, List<object> Keys) : QueueItem
    {

    }

    public class Decider
    {
        private IDecisionTreeX? current;

        public IDecisionTreeX? Current
        {
            get { return current; }
            set
            {
                if (current != null)
                    current.IsInvoked = false;
                current = value;
                if (current != null)
                    current.IsInvoked = true;
            }
        }

        public static Decider Instance { get; } = new();
    }

}
