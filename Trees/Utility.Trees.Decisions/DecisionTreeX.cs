using System.Text;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Pipes;

namespace Utility.Trees.Decisions
{
    public class DecisionTreeX : DecisionTree<decimal>
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


        private readonly Func<IEnumerable<object>, object?> combine;

        public DecisionTreeX(IDecision decision, string key, Func<decimal, object>? transform = null, Func<IEnumerable<object>, object?>? combine = null) : base(decision, transform)
        {
            Key = key;
            this.combine = combine ?? new Func<IEnumerable<object>, object?>(a =>
        {
            return a.FirstOrDefault(v => v != null);
        });
        }

        public void Evaluate(List<string> keys)
        {

            var result = decision.Evaluate(Input);

            if (result == Match)
            {
                Output = Transform(Input);
                if (Items.Any())
                {
                    foreach (DecisionTreeX item in Items)
                    {
                        item.Parent = this;
                        item.Input = Output;
                        var queueItem = new ForwardItem(item, Output, keys);

                        Pipe.Instance.Queue(queueItem);
                    }
                }
                else if (this.Parent is DecisionTreeX x)
                {
                    Backput = this.Output;
                    Pipe.Instance.Queue(new BackItem(x, Backput, keys));
                }
                return;
            }
        }


        public void BackPropagate(List<string> keys)
        {
            List<object> outputs = new();

            foreach (DecisionTree item in Items)
            {
                outputs.Add(item.Output);
            }

            Backput = combine.Invoke(outputs);
            if (this.Parent is DecisionTreeX x)
            {
                var queueItem = new BackItem(x, Backput, keys);
                Pipe.Instance.Queue(queueItem);
            }
        }

    }

    public record ForwardItem(DecisionTreeX Tree, object Value, List<string> Keys) : DecisionQueueItem(Tree, Value, Keys)
    {
        public override void Invoke()
        {
            Decider.Instance.Current = Tree;
            Keys.Add(Tree.Key);
            Tree.Evaluate(Keys);
        }
    }



    public record BackItem(DecisionTreeX Tree, object Value, List<string> Keys) : DecisionQueueItem(Tree, Value, Keys)
    {
        public override void Invoke()
        {
            Decider.Instance.Current = Tree;
            Keys.Add(Tree.Key);
            Tree.BackPropagate(Keys);
        }
    }

    public abstract record DecisionQueueItem(DecisionTreeX Tree, object Value, List<string> Keys) : QueueItem
    {

    }

    public class Decider
    {
        private DecisionTreeX current;

        public DecisionTreeX Current
        {
            get { return current; }
            set
            {
                if (current != null)
                    current.IsInvoked = false;
                current = value;
                current.IsInvoked = true;
            }
        }

        public static Decider Instance { get; } = new();
    }

}
