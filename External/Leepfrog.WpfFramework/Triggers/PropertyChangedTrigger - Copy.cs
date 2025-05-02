using Leepfrog.WpfFramework.Converters;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Triggers
{
    // ItemsSourceDataTrigger - evaluates binding on each item in itemssource
    public class ItemsSourceDataTrigger : Microsoft.Xaml.Behaviors.Core.DataTrigger
    {


        private MultiBinding _multiBinding;
        

        public ItemsSourceDataTrigger() : base()
        {
            hookItemsSource(null);            
        }
        

        // ItemsSource is an IEnumerable
        // ItemBinding is a binding to be evaluated on each item in ItemsSource
        // we need to attach to collection changes on ItemsSource
        // and bind a new binding to each item



        /// <summary>Backing DP for the ItemsSource property</summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsSourceDataTrigger), new PropertyMetadata(itemsSource_Changed));

        

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }        

        public enum MatchCondition
        {
            Any,
            All
        }


        public MatchCondition Match
        {
            get { return (MatchCondition)GetValue(MatchProperty); }
            set { SetValue(MatchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MatchType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MatchProperty =
            DependencyProperty.Register("Match", typeof(MatchCondition), typeof(ItemsSourceDataTrigger), new PropertyMetadata(MatchCondition.Any,Match_Changed));

        private static void Match_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var typed = d;
        }

        

        public object ItemBinding
        {
            get { return GetValue(ItemBindingProperty); }
            set { SetValue(ItemBindingProperty, value); }
        }

        private BindingBase _itemBinding
        {
            get
            {
                Binding b = BindingOperations.GetBinding(this, ItemsSourceDataTrigger.ItemBindingProperty);
                return b;
            }
        }

        // Using a DependencyProperty as the backing store for ItemBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemBindingProperty =
            DependencyProperty.Register("ItemBinding", typeof(object), typeof(ItemsSourceDataTrigger), new PropertyMetadata(null, itemBinding_Changed));

        private static void itemBinding_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // todo: rebuild each item in multibinding using this new binding
            var typedItem = d as ItemsSourceDataTrigger;
            typedItem.updateBindings();
        }

        private void updateBindings()
        {
            foreach (var binding in _multiBinding.Bindings)
            {
                updateBinding(binding);
            }
        }

        private BindingBase createBinding(object source)
        {
            BindingBase newBinding = null;
            if (_itemBinding is Binding)
            {
                newBinding =
                    new Binding()
                    {
                        Source = source,
                        BindsDirectlyToSource=true,
                        NotifyOnSourceUpdated=true
                    };
                updateBinding(newBinding);
            }
            return newBinding;
        }

        private void updateBinding(BindingBase binding)
        {
            if (_itemBinding is Binding)
            {
                var thisBinding = _itemBinding as Binding;
                var typedBinding = binding as Binding;
                typedBinding.FallbackValue = thisBinding.FallbackValue;
                typedBinding.Mode = thisBinding.Mode;
                typedBinding.Path = thisBinding.Path;
                typedBinding.StringFormat = thisBinding.StringFormat;
                typedBinding.TargetNullValue = thisBinding.TargetNullValue;
            }
        }

        private static void itemsSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var typedObject = (d as ItemsSourceDataTrigger);
            typedObject.unhookItemsSource((IEnumerable)e.OldValue);
            typedObject.hookItemsSource((IEnumerable)e.NewValue);
        }

        private void unhookItemsSource(IEnumerable oldItemsSource)
        {
            if (oldItemsSource != null)
            {
                // unhook the collection
                (oldItemsSource as INotifyCollectionChanged).CollectionChanged -= itemsSource_CollectionChanged;
            }
            // remove all existing bindings
            BindingOperations.ClearBinding(this, BindingProperty);
            _multiBinding = null;
        }

        private void hookItemsSource(IEnumerable newItemsSource)
        {
            _multiBinding = new MultiBinding()
            {
                Converter = new ItemsSourceMultiConverter(),
                ConverterParameter = this,
                NotifyOnSourceUpdated = true
            };
            if (ItemsSource != null)
            {
                BindingOperations.AccessCollection(
                    newItemsSource,
                    () =>
                    {
                        //while the list is locked, we can create a binding for each existing item
                        foreach (var item in newItemsSource)
                        {
                            _multiBinding.Bindings.Add(createBinding(item));
                        }
                        (newItemsSource as INotifyCollectionChanged).CollectionChanged += itemsSource_CollectionChanged;
                    }, false);
            }
            BindingOperations.SetBinding(this, BindingProperty, _multiBinding);
        }

        private void itemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // unfortunately, we just have to kill and recreate
            unhookItemsSource(ItemsSource);
            hookItemsSource(ItemsSource);
        }


        protected override void OnAttached()
        {
            base.OnAttached();
            var element = AssociatedObject as FrameworkElement;
            if (element != null)
            {
                element.Loaded += OnElementLoaded;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            var element = AssociatedObject as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= OnElementLoaded;
            }
        }

        private void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            EvaluateBindingChange(null);
        }

        protected override void EvaluateBindingChange(object args)
        {
            base.EvaluateBindingChange(args);
        }

        internal object Evaluate(object[] values)
        {
            object ret = null;
            foreach (var value in values)
            {
                ret = value;
                string val1 = value.ToString();
                string val2 = Value.ToString();
                bool pass = false;
                switch (Comparison)
                {
                    case ComparisonConditionType.NotEqual:
                        pass = (val1 != val2);
                        break;
                    case ComparisonConditionType.Equal:
                        pass = (val1 == val2);
                        break;
                        /*
                    case ComparisonConditionType.GreaterThan:
                        pass = ((double)value > (double)Value);
                        break;
                    case ComparisonConditionType.LessThan:
                        pass = ((double)value < (double)Value);
                        break;
                    case ComparisonConditionType.GreaterThanOrEqual:
                        pass = ((double)value >= (double)Value);
                        break;
                    case ComparisonConditionType.LessThanOrEqual:
                        pass = ((double)value <= (double)Value);
                        break;
                        */
                }
                // if we need to match all, and this one doesn't match...
                if ((Match==MatchCondition.All) && (!pass))
                {
                    // we'll return this one, because it's a fail!
                    break;
                }
                // if we're happy to match any, and this one is a match...
                if ((Match==MatchCondition.Any) && (pass))
                {
                    // we'll return this one, because it's a pass!
                    break;
                }
            }
            // if we didn't break out of the loop, we will just return the final item
            // if we were looking for any, we must have failed, so the final item must be a fail
            // if we were looking for all, we must have found all, so the final item must be a pass
            return ret;
        }


        class DummyDO : DependencyObject
        {
            public object Value
            {
                get { return (object)GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(DummyDO), new UIPropertyMetadata(null));

        }

        public object EvalBinding(BindingBase b)
        {
            // TODO: Put DummyDO into a global class
            // TODO: Make this a one-time only binding?
            DummyDO d = new DummyDO();
            BindingOperations.SetBinding(d, DummyDO.ValueProperty, b);
            var value = d.Value;
            BindingOperations.ClearBinding(d, DummyDO.ValueProperty);
            return value;
        }


    }
}

