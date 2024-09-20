using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.Helpers.NonGeneric;
using System.Reactive.Linq;
using Utility.PropertyNotifications;
using NetFabric.Hyperlinq;
using Utility.Reactives;

namespace Utility.Trees.Demo.MVVM
{


    public class TreeViewItemDecisionTree : DecisionTree<object, TreeViewItem>
    {
        public TreeViewItemDecisionTree(IDecision decision, Func<object, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }
    }


    public class StringDecisionTree<T> : DecisionTree<T, string>
    {
        public StringDecisionTree(IDecision decision, Func<T, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }
    }
    public class StringDecisionTree : DecisionTree
    {
        public StringDecisionTree(IDecision decision, Func<object, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }

        protected override object ToBackPut(ICollection<object> backputs)
        {
            return backputs.FirstOrDefault(a => a is string);
        }
    }




    public class DecisionTree<T, TR> : DecisionTree<T>
    {
        public DecisionTree(IDecision decision, Func<T, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }

        protected override object ToBackPut(ICollection<object> backputs)
        {
            return backputs.FirstOrDefault(a => a is TR);
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


    public class BooleanDecisionTree<T> : DecisionTree<T>
    {

        public BooleanDecisionTree(IDecision decision, Func<T, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }

        protected override object ToBackPut(ICollection<object> backputs)
        {
            if (AndOr is AndOr.And)
            {
                return backputs.All(a => a is bool b ? b : true);
            }
            if (AndOr is AndOr.Or)
            {
                return backputs.Any(a => a is bool b ? b : true);
            }
            throw new Exception(" £3344333");
        }
    }

    public class BooleanDecisionTree : DecisionTree
    {
        public BooleanDecisionTree(IDecision decision, Func<object, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
        }

        protected override object ToBackPut(ICollection<object> backputs)
        {
            if (AndOr is AndOr.And)
            {
                return backputs.All(a => a is bool b ? b : true);
            }
            if (AndOr is AndOr.Or)
            {
                return backputs.Any(a => a is bool b ? b : true);
            }
            throw new Exception(" £3344333");
        }
    }

    public abstract class DecisionTree<T> : DecisionTree
    {
        private readonly Func<T, object>? transform;

        public DecisionTree(IDecision decision, Func<T, object>? transform = null) : base(decision)
        {
            Data = decision;
            this.transform = transform ?? new Func<T, object>(a => a);
        }

        protected override object Transform(object value)
        {
            if (value is not T t)
            {
                throw new Exception("fds df33 dffd");
            }

            return transform.Invoke(t);
        }

    }

    public class DecisionTree : Tree
    {
        private readonly IDecision decision;
        private object input;
        private object output;
        private object backput;
        private Func<object, object>? transform;

        public DecisionTree(IDecision decision, Func<object, object>? transform = null, bool match = true) 
        {
            this.decision = decision;
            this.Match = match;
            this.transform = transform ?? new Func<object, object>(a => a);
        }

        public AndOr AndOr { get; set; }


        public void Evaluate()
        {
            List<object> outputs = new();

            //var compile = (Data as IDecision).Predicate.Compile().Invoke(Input);

            var result = decision.Evaluate(Input);

            if (result == Match)
            {
                Output = Transform(Input);
                if (Items.Any())
                {
                    foreach (DecisionTree item in Items)
                    {
                        item.Input = Output;
                        item.Evaluate();
                        outputs.Add(item.Backput);
                    }
                    Backput = ToBackPut(outputs);
                }
                else
                {
                    Backput = Output;
                }
                return;
            }
            Backput = null;
        }

        public void Reset()
        {
            Input = null;
            Output = null;
            Backput = null;
            if (Items.Any())
            {
                foreach (DecisionTree x in Items)
                {
                    x.Reset();
                }
            }
        }


        public void Eval()
        {
            List<object> outputs = new();

            //var compile = (Data as IDecision).Predicate.Compile().Invoke(Input);

            var result = decision.Evaluate(Input);

            if (result == Match)
            {
                Output = Transform(Input);
                if (Items.Any())
                {
                    Items.Cast<DecisionTree>()
                        .Select(x =>
                        {
                            var xe = x.WithChangesTo(a => a.Backput);
                            x.Input = Output;
                            Pipe.Instance.Queue(new DecisionTreeQueueItem(x));
                            return xe;
                        })
                        .Combine()
                        .Subscribe(outputs =>
                        {
                            Backput = ToBackPut(outputs);
                        });
                }
                else
                {
                    Backput = Output;
                }
         
                return;
            }
            Backput = null;
        }

        protected virtual object Transform(object value)
        {
            if (transform != null)
                return transform.Invoke(value);
            throw new Exception(" ew332 324");
        }

        protected virtual object ToBackPut(ICollection<object> backputs)
        {
            return backputs.SingleOrDefault(a => a != null);
        }


        public object Input
        {
            get => input; set
            {
                input = value;
                RaisePropertyChanged();
            }
        }
        public object Output
        {
            get => output; set
            {
                output = value;
                RaisePropertyChanged();
            }
        }
        public object Backput
        {
            get => backput; set
            {
                backput = value;
                RaisePropertyChanged();
            }
        }


        public bool Match { get; }
    }
}
