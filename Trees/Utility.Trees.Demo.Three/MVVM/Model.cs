using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Utility.PropertyNotifications;
using Utility.WPF.Controls.Trees;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.Helpers.NonGeneric;
using Utility.Infrastructure;

namespace Utility.Trees.Demo.MVVM
{
    public partial record Model : NotifyProperty
    {
        private Type type;

        public Type Type
        {
            get
            {

                this.RaisePropertyCalled(type);
                return type;

            }
            set
            {
                if (value.Equals(type))
                    return;
                type = value;
                this.RaisePropertyReceived(value);
            }
        }

        public bool IsReadOnly { get; internal set; }

        public class StyleSelector : System.Windows.Controls.StyleSelector
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri($"/{typeof(CustomTreeViewItem).Namespace};component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            public override Style SelectStyle(object item, DependencyObject container)
            {
                if (item is TreeViewItem { })
                {
                    var style = resourceDictionary["ButtonsAddRemoveStyle"] as Style;
                    return style;
                }
                return base.SelectStyle(item, container);
            }

            //public ResourceDictionary NewTemplates => new()
            //{
            //    Source = new Uri($"/{typeof(CustomStyleSelector).Assembly.GetName().Name};component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            //};

            public static StyleSelector Instance { get; } = new();
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

        protected override object ToBackPut(List<object> backputs)
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

        protected override object ToBackPut(List<object> backputs)
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

        protected override object ToBackPut(List<object> backputs)
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

        protected override object ToBackPut(List<object> backputs)
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

        protected override object ToBackPut(List<object> backputs)
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
            this.transform = transform;
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

    public abstract class DecisionTree : Tree
    {
        private readonly IDecision decision;
        private object input;
        private object output;
        private object backput;
        private Func<object, object>? transform;

        public DecisionTree(IDecision decision, bool match = true)
        {
            Data = decision;
            this.decision = decision;
            Match = match;
        }

        protected DecisionTree(IDecision decision, Func<object, object>? transform, bool match = true) : this(decision, match)
        {
            this.decision = decision;
            this.transform = transform ?? new Func<object, object>(a => a);
        }

        public AndOr AndOr { get; set; }


        public void Evaluate()
        {
            Backput = eval();
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


        private object eval()
        {
            List<object> outputs = new();

            //var compile = (Data as IDecision).Predicate.Compile().Invoke(Input);

            var result = decision.Evaluate(Input);

            if (result == Match)
            {
                Output = Transform(Input);
                if (Items.Any())
                {
                    foreach (DecisionTree x in Items)
                    {
                        x.Input = Output;
                        outputs.Add(x.eval());
                    }
                    return Backput = ToBackPut(outputs);
                }
                return Output;
            }
            return null;
        }

        protected virtual object Transform(object value)
        {
            if (transform != null)
                return transform.Invoke(value);
            throw new Exception(" ew332 324");
        }

        protected abstract object ToBackPut(List<object> backputs);


        public object Input
        {
            get => input; set
            {
                input = value;
                OnPropertyChanged();
            }
        }
        public object Output
        {
            get => output; set
            {
                output = value;
                OnPropertyChanged();
            }
        }
        public object Backput
        {
            get => backput; set
            {
                backput = value;
                OnPropertyChanged();
            }
        }


        public bool Match { get; }
        //public Decision<IDescriptor> Decision { get; }
    }
}
