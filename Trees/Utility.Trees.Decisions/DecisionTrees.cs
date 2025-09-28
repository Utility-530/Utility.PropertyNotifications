using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Enums;

namespace Utility.Trees.Decisions
{


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

        protected override object? ToBackPut(ICollection<object?> backputs)
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

        protected override object? ToBackPut(ICollection<object?> backputs)
        {
            return backputs.FirstOrDefault(a => a is TR);
        }
    }


    public class TrueDecisionTree<T> : BooleanDecisionTree<T>
    {
        public TrueDecisionTree(IDecision decision, Func<T, object>? transform = null) : base(decision, true, transform)
        {
            Data = decision;
        }
    }

    public class FalseDecisionTree<T> : BooleanDecisionTree<T>
    {
        public FalseDecisionTree(IDecision decision, Func<T, object>? transform = null) : base(decision, false, transform)
        {
            Data = decision;
        }
    }
     
    public class BooleanDecisionTree<T> : DecisionTree<T>
    {

        public BooleanDecisionTree(IDecision decision, bool defaultValue, Func<T, object>? transform = null) : base(decision, transform)
        {
            Data = decision;
            DefaultValue = defaultValue;
        }

        public AndOr AndOr { get; set; }
        public bool DefaultValue { get; }

        protected override object? ToBackPut(ICollection<object?> backputs)
        {
            if (AndOr is AndOr.And)
            {
                return backputs.All(a => a is bool b ? b : DefaultValue);
            }
            if (AndOr is AndOr.Or)
            {
                return backputs.Any(a => a is bool b ? b : DefaultValue);
            }
            throw new Exception(" £3344333");
        }
    }

    public class TrueDecisionTree : BooleanDecisionTree
    {
        public TrueDecisionTree(IDecision decision, Func<object, object>? transform = null) : base(decision, true, transform)
        {
            Data = decision;
        }

    }
    public class FalseDecisionTree : BooleanDecisionTree
    {
        public FalseDecisionTree(IDecision decision, Func<object, object>? transform = null) : base(decision, false, transform)
        {
            Data = decision;
        }

    }

    public class BooleanDecisionTree : DecisionTree
    {
        public BooleanDecisionTree(IDecision decision, bool defaultValue, Func<object, object?>? transform = null) : base(decision, transform)
        {
            Data = decision;
            DefaultValue = defaultValue;
        }

        public AndOr AndOr { get; set; }

        public bool DefaultValue { get; }

        protected override object? ToBackPut(ICollection<object?> backputs)
        {
            if (AndOr is AndOr.And)
            {
                return backputs.All(a => a is bool b ? b : DefaultValue);
            }
            if (AndOr is AndOr.Or)
            {
                return backputs.Any(a => a is bool b ? b : DefaultValue);
            }
            throw new Exception(" £3344333");
        }
    }

    public class DecisionTree<T> : DecisionTree
    {
        private readonly Func<T, object?> transform;

        public DecisionTree(IDecision decision, Func<T, object?>? transform = null) : base(decision)
        {
            Data = decision;
            this.transform = transform ?? new Func<T, object?>(a => a);
        }

        protected override object? Transform(object? value)
        {
            if (value is not T t)
            {
                //throw new Exception("fds df33 dffd");
                return null; 
            }

            return transform.Invoke(t);
        }

    }

    public class DecisionTree : Tree
    {
        protected readonly IDecision decision;
        private object? input;
        private object? output;
        private object? backput;
        private readonly Func<object, object?> transform;
        private bool hasEvaluated;

        public DecisionTree(IDecision decision, Func<object, object?>? transform = null, bool match = true)
        {
            this.decision = decision;
            Match = match;
            this.transform = transform ?? new Func<object, object?>(a => a);
        }


        public virtual void Evaluate()
        {
            List<object?> outputs = new();

            //var compile = (Data as IDecision).Predicate.Compile().Invoke(Input);

            var result = decision.Evaluate(Input);
            HasEvaluated = true;
            if (result == Match)
            {
                Output = Transform(Input);
                if (Output == null)
                    return;
                if (Children.Any())
                {
                    foreach (DecisionTree item in Children)
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
            if (Children.Any())
            {
                foreach (DecisionTree x in Children)
                {
                    x.Reset();
                }
            }
        }

        protected virtual object? Transform(object? value)
        {
            if (transform != null)
                return transform.Invoke(value);
            throw new Exception(" ew332 324");
        }

        protected virtual object? ToBackPut(ICollection<object?> backputs)
        {
            return backputs.SingleOrDefault(a => a != null);
        }


        public object? Input
        {
            get => input; set
            {
                input = value;
                HasEvaluated = false;
                RaisePropertyChanged();
            }
        }

        public bool HasEvaluated
        {
            get => hasEvaluated; set
            {
                hasEvaluated = value;
                RaisePropertyChanged();
            }
        }

        public object? Output
        {
            get => output; set
            {
                output = value;
                RaisePropertyChanged();
            }
        }

        public object? Backput
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
