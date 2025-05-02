using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;


namespace Leepfrog.WpfFramework
{
    public class IfListMultiBinding : MultiBinding
    {
        public IfListMultiBinding()
        {
            
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //this.AddLog("IFLIST", "coll Changed");
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    foreach ( var item in e.NewItems )
                    {
                        hookItem(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // DO NOTHING!
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach ( var item in e.OldItems )
                    {
                        unhookItem(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("IfListExtension.Items_CollectionChanged with Replace is not expected.");
                //break;
                case NotifyCollectionChangedAction.Reset:
                    for ( var i = _dummyDOs.Count() - 1; i >= 0; i-- )
                    {
                        var d = _dummyDOs[i];
                        unhookItem(BindingOperations.GetBinding(d, DummyDO.ValueProperty).Source);
                    }
                    foreach ( var item in (sender as IEnumerable) )
                    {
                        hookItem(item);
                    }
                    break;
            }
            // FORCE UPDATE!
            anyBinding_Changed(null, null);
        }


        internal PropertyPath _path;
        private List<DummyDO> _dummyDOs = new List<DummyDO>();
        private void hookItem(object item)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => hookItem(item));
                return;
            }
            //this.AddLog("IFLIST", $"hooking {item}[{item.GetHashCode():X8}] . {_path?.Path}");
            // CREATE A BINDING USING GIVEN PATH WITH THIS OBJECT AS SOURCE
            var binding = new Binding();
            binding.Path = _path;
            binding.Source = item;
            DummyDO d = new DummyDO();
            _dummyDOs.Add(d);
            BindingOperations.SetBinding(d, DummyDO.ValueProperty, binding);
            d.Changed += anyBinding_Changed;
        }

        private void anyBinding_Changed(object sender, EventArgs e)
        {
            //this.AddLog("IFLIST", $"updating target '{ _targetObject }'.'{ _targetProperty?.Name }'");
            var expr = BindingOperations.GetBindingExpressionBase(_targetObject, _targetProperty);
            Application.Current.Dispatcher.BeginInvoke(new Action(() => expr?.UpdateTarget()),System.Windows.Threading.DispatcherPriority.DataBind,null);
        }

        private void unhookItem(object item)
        {
            //this.AddLog("IFLIST", "unhooking");
            var DOtoRemove = _dummyDOs.FirstOrDefault(d => BindingOperations.GetBinding(d, DummyDO.ValueProperty).Source == item);
            if ( DOtoRemove != null )
            {
                _dummyDOs.Remove(DOtoRemove);
            }
        }

        internal void recordTarget(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            _targetObject = targetObject;
            _targetProperty = targetProperty;
        }

        internal DependencyObject _targetObject;
        internal DependencyProperty _targetProperty;

        private INotifyCollectionChanged _coll;

        internal void setCollection(INotifyCollectionChanged coll)
        {
            var oldColl = _coll;
            if ( oldColl == coll )
            {
                return;
            }
            _coll = coll;
            if ( oldColl != null )
            {
                //this.AddLog("IFLIST", "unsetting old collection");
                //changed ml 2018-07-15
                //use a weak event handler to avoid memory leaks
                //oldColl.CollectionChanged -= Items_CollectionChanged;
                CollectionChangedEventManager.RemoveHandler(oldColl, Items_CollectionChanged);
                foreach ( var item in oldColl as IEnumerable )
                {
                    unhookItem(item);
                }
            }
            if (_coll != null)
            {
                //this.AddLog("IFLIST", "setting collection");
                foreach ( var item in _coll as IEnumerable )
                {
                    hookItem(item);
                }
                //changed ml 2018-07-15
                //use a weak event handler to avoid memory leaks
                //_coll.CollectionChanged += Items_CollectionChanged;
                CollectionChangedEventManager.AddHandler(_coll, Items_CollectionChanged);
            }
        }


        class DummyDO : DependencyObject
        {
            public event EventHandler Changed;
            public object Value
            {
                get { return (object)GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(DummyDO), new UIPropertyMetadata(null, valueChangedCallback));

            private static void valueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                //d.AddLog("DepObj", "DepObj Value_Changed");
                ( d as DummyDO ).Changed?.Invoke(d, EventArgs.Empty);
            }
        }


    }
}
