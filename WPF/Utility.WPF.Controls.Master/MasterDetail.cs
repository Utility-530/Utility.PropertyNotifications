using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;
using ItemsControl = System.Windows.Controls.ItemsControl;

namespace Utility.WPF.Controls.Master
{
    using System.Reactive.Subjects;
    using System.Windows.Controls.Primitives;
    using Evan.Wpf;
    using ReactiveUI;
    using Utility.Reactives;
    using Utility.WPF.Abstract;
    using Utility.WPF.Controls.Base;
    using Utility.WPF.Helpers;
    using Utility.WPF.Reactives;

    /// <summary>
    /// Only transforms master-list items to the detail-item; and not vice-versa
    /// </summary>
    public class ReadOnlyMasterDetail : SelectorAndContentControl
    {
        public static readonly DependencyProperty ConverterProperty = DependencyHelper.Register<IValueConverter>();
        public static readonly DependencyProperty ConverterParameterProperty = DependencyHelper.Register<object>();
        public static readonly DependencyProperty PropertyKeyProperty = DependencyHelper.Register<string>();
        public static readonly DependencyProperty UseDataContextProperty = DependencyHelper.Register<bool>();

        private readonly ReplaySubject<object> dataContextSubject = new(1);

        static ReadOnlyMasterDetail()
        {
            //   DefaultStyleKeyProperty.OverrideMetadata(typeof(ReadOnlyMasterDetail), new FrameworkPropertyMetadata(typeof(ReadOnlyMasterDetail)));
            DataContextProperty.OverrideMetadata(typeof(ReadOnlyMasterDetail), new FrameworkPropertyMetadata(null, Changed));
        }

        public ReadOnlyMasterDetail()
        {
            _ = this
                .WhenAnyValue(a => a.Content)
                .WhereNotNull()
                .Select(a => this.Selector)
                .WhereNotNull()
                .CombineLatest(dataContextSubject)
                .Subscribe(c =>
                {
                    var (first, second) = c;
                    first.DataContext = second;
                });

            TransformObservable = ObservableHelper.ToReplaySubject(Transform(
                this.WhenAnyValue(a => a.Selector)
                    .WhereNotNull()
                    .ObserveOnDispatcher()
                    .Select(SelectFromMaster)
                    .Switch(),
                this.WhenAnyValue(a => a.Converter),
                this.WhenAnyValue(a => a.ConverterParameter),
                this.WhenAnyValue(a => a.PropertyKey)));

            _ = TransformObservable
                .Select(a => a.New)
                .Subscribe(content =>
                {
                    SetDetail(Content, content);
                });

            //_ = this.WhenAnyValue(a => a.Orientation)
            //    .CombineLatest(this.WhenAnyValue(a => a.Selector))
            //    .Subscribe(combined =>
            //    {
            //        var (first, second) = combined;
            //        if (second is IOrientation orientation)
            //        {
            //            orientation.Orientation = (Orientation)(((int)first + 1) % ((int)Orientation.Vertical + 1));
            //        }
            //});
        }

        public override event SelectionChangedEventHandler SelectionChanged
        {
            add
            {
                switch (Selector)
                {
                    case ISelector selector:
                        selector.SelectionChanged += value;
                        break;

                    case Selector selector:
                        selector.SelectionChanged += value;
                        break;

                    default: throw new ApplicationException($"Unexpected type, {Selector.GetType().Name}, for {nameof(Selector)} ");
                }
                ;
            }

            remove
            {
                switch (Selector)
                {
                    case ISelector selector:
                        selector.SelectionChanged -= value;
                        break;

                    case Selector selector:
                        selector.SelectionChanged -= value;
                        break;

                    default: throw new ApplicationException($"Unexpected type, {Selector.GetType().Name}, for {nameof(Selector)} ");
                }
                ;
            }
        }

        #region properties

        public IValueConverter Converter
        {
            get => (IValueConverter)GetValue(ConverterProperty);
            set => SetValue(ConverterProperty, value);
        }

        public object ConverterParameter
        {
            get => (object)GetValue(ConverterParameterProperty);
            set => SetValue(ConverterParameterProperty, value);
        }

        public Control? Selector => Content as Control;

        // set { SetValue(SelectorProperty, value); }
        public string PropertyKey
        {
            get => (string)GetValue(PropertyKeyProperty);
            set => SetValue(PropertyKeyProperty, value);
        }

        public bool UseDataContext
        {
            get => (bool)GetValue(UseDataContextProperty);
            set => SetValue(UseDataContextProperty, value);
        }

        #endregion properties

        protected IObservable<TransformProduct?> TransformObservable { get; }

        //object ISelector.SelectedItem
        //{
        //    get
        //    {
        //        return Selector switch
        //        {
        //            ISelector selector => selector.SelectedItem,
        //            Selector selector => selector.SelectedItem,
        //            _ => throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} "),
        //        };
        //    }
        //}

        //int ISelector.SelectedIndex
        //{
        //    get
        //    {
        //        return Selector switch
        //        {
        //            ISelector selector => selector.SelectedIndex,
        //            Selector selector => selector.SelectedIndex,
        //            _ => throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} "),
        //        };
        //    }
        //}

        //IEnumerable ISelector.ItemsSource
        //{
        //    get
        //    {
        //        return Selector switch
        //        {
        //            ISelector selector => selector.ItemsSource,
        //            Selector selector => selector.ItemsSource,
        //            _ => throw new ApplicationException($"Unexpected type,{Selector.GetType().Name} for {nameof(Selector)} "),
        //        };
        //    }
        //}

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ReadOnlyMasterDetail readOnly)
                return;

            if (e.NewValue is object obj)
                d.Dispatcher.InvokeAsync(() => readOnly.dataContextSubject.OnNext(obj));
        }

        /// <summary>
        /// Gets the selections made in the master-list
        /// </summary>
        protected virtual IObservable<object> SelectFromMaster(Control control)
        {
            return control switch
            {
                ISelector selector => selector.Changes(true),
                Selector selector => selector.Changes(true),
                _ => throw new ApplicationException($"Unexpected type,{control.GetType().Name} for {nameof(Selector)} "),
            };
        }

        /// <summary>
        /// Updates the detail-item with changes made to the  master-list
        /// </summary>
        protected virtual void SetDetail(object content, object? masterObject)
        {
            if (UseDataContext)
            {
                if (Header is FrameworkElement frameworkElement)
                {
                    frameworkElement.DataContext = masterObject;
                    return;
                }
                else if (masterObject is FrameworkElement frameworkElement1)
                {
                    Header = frameworkElement1.DataContext;
                }
                else
                {
                    //throw new ApplicationException("Content needs to be framework element is UseDataContext set to true");
                }
            }
            if (masterObject is IEnumerable enumerable and not string)
            {
                switch (Header)
                {
                    case IItemsSource oview:
                        oview.ItemsSource = enumerable;
                        return;

                    case ItemsControl itemsControl:
                        itemsControl.ItemsSource = enumerable;
                        return;

                    default:
                        return;
                }
            }
            //else if (content is JsonView propertyGrid)
            //{
            //    propertyGrid.Object = propertyGrid;
            //}
            if (masterObject is UIElement uiElement)
            {
                Header = Extract(uiElement);
            }
            //else if (headerPresenter != null)
            //{
            //    headerPresenter.Content = masterObject;
            //}
            else
            {
                //throw new Exception("dgsf33 gggdsfsd");
            }
        }

        private object Extract(UIElement uiElement)
        {
            if (uiElement is HeaderedContentControl headeredContentControl)
            {
                if (headeredContentControl.Content is FrameworkElement dependencyObject)
                {
                    dependencyObject.DetachFromParent();
                }
                return headeredContentControl.Content;
            }
            else if (uiElement is HeaderedItemsControl headeredItemsControl)
            {
                var array = headeredItemsControl.ItemsSource;
                headeredItemsControl.ItemsSource = null;
                return new ItemsControl { ItemsSource = array };
            }
            try
            {
                return CloneHelper.XamlClone(uiElement);
            }
            catch (Exception ex)
            {
                //Header = Force.DeepCloner.DeepClonerExtensions.DeepClone(masterObject);
                if (uiElement is FrameworkElement frameworkElement)
                    return frameworkElement.DataContext;
                else
                    throw;
            }
        }

        protected record TransformProduct(object? New, object? Old, IValueConverter? Converter, object ConverterParameter, string? DataKey);

        /// <summary>
        /// Makes any changes to selected-item before becoming the detail item
        /// </summary>
        protected static IObservable<TransformProduct?> Transform(
            IObservable<object> collectionViewModel,
            IObservable<IValueConverter> dataConversions,
            IObservable<object> converterParameters,
            IObservable<string> dataKeys)
        {
            return collectionViewModel
                .CombineLatest(dataConversions, converterParameters, dataKeys)
                .ObserveOnDispatcher()
                .Scan(default(TransformProduct), (a, b) =>
                {
                    var (selected, converter, converterParameter, dataKey) = b;

                    //if (selected == null)
                    //    throw new Exception("ds009fsd");
                    object? newItem = Convert(selected, converter, converterParameter, dataKey);

                    return new TransformProduct(newItem, selected, converter, converterParameter, dataKey);
                });
        }

        protected static object? Convert(object selected, IValueConverter? converter, object? converterParameter, string? dataKey)
        {
            return (converter, dataKey) switch
            {
                ({ } conv, _) => conv.Convert(selected, default, converterParameter, default),
                (_, { } key) => PropertyConverter.Convert(selected, key),
                (null, null) => selected
            };
        }

        protected class PropertyConverter : IConverter
        {
            public static object? Convert(object selected, string key)
            {
                return Utility.Helpers.Reflection.PropertyHelper.GetPropertyRefValue<object>(selected, key);
            }

            public static object ConvertBack(object selected, string k, object selectedValueOld)
            {
                Utility.Helpers.Reflection.PropertyHelper.SetValue(selected, k, selectedValueOld);
                return selectedValueOld;
            }
        }
    }

    public class MasterDetail : ReadOnlyMasterDetail
    {
        static MasterDetail()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterDetail), new FrameworkPropertyMetadata(typeof(MasterDetail)));
        }

        public MasterDetail()
        {
            Transform(TransformObservable);
        }

        protected static void Transform(IObservable<TransformProduct?> transforms)
        {
            //transforms
            //     .WhereNotNull()
            //     .Scan((default(TransformProduct), default(TransformProduct)), (a, b) => (a.Item2, b))
            //     .Select(a => a.Item1)
            //     .WhereNotNull()
            //     .Where(a => a.Old != null)
            //     .Subscribe(tp =>
            //     {
            //         object? replacement = null;

            //         replacement = (tp.New, tp.Converter, tp.DataKey) switch
            //         {
            //             (null, _, _) => null,
            //             (object o, IValueConverter conv, _) => conv.ConvertBack(o, default, tp.ConverterParameter, default),
            //             (object o, _, string key) => PropertyConverter.ConvertBack(tp.Old!, key, o),
            //             (object o, null, null) => o
            //         };

            //         if (replacement == null)
            //             throw new Exception("7788dfdfgf");
            //         //if (tp.Old != null && tp.Old == replacement && tp.Old != null)
            //         //    throw new ApplicationException("selectedOld and ee can't be the same object in order to compare them after conversion.");

            //         PropertyMerger.Instance.Set(tp.Old!, replacement);
            //     });
        }
    }
}