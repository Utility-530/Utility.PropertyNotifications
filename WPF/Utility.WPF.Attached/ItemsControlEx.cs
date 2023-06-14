using DynamicData.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Enums;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.WPF.Panels.Helpers;
using Orientation = System.Windows.Controls.Orientation;

namespace Utility.WPF.Attached
{
    public partial class ItemsControlEx : ItemsControl
    {
        #region AlternateTemplate

        public static readonly DependencyProperty AlternateTemplateProperty = DependencyProperty.RegisterAttached("AlternateTemplate", typeof(DataTemplate), typeof(ItemsControlEx), new PropertyMetadata(null, AlternateTemplateChange));

        public static object GetAlternateTemplate(DependencyObject d)
        {
            return d.GetValue(AlternateTemplateProperty);
        }

        public static void SetAlternateTemplate(DependencyObject d, DataTemplate value)
        {
            d.SetValue(AlternateTemplateProperty, value);
        }

        private static void AlternateTemplateChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
            {
                DataTemplate originalTemplate = itemsControl.ItemTemplate;
                _ = itemsControl.Items.ObserveCollectionChanges().Select(a => itemsControl.Items.Count)
                    .Subscribe(count =>
                    {
                        if (count == 1)
                        {
                            originalTemplate = itemsControl.ItemTemplate;
                            itemsControl.ItemTemplate = GetAlternateTemplate(d) as DataTemplate;
                        }
                        else
                            itemsControl.ItemTemplate = originalTemplate;
                    });
            }
        }

        #endregion AlternateTemplate

        #region NewItem

        public static readonly DependencyProperty NewItemProperty = DependencyProperty.RegisterAttached("NewItem", typeof(object), typeof(ItemsControlEx), new PropertyMetadata(null, NewItemChange));

        public static object GetNewItem(DependencyObject d)
        {
            return d.GetValue(NewItemProperty);
        }

        public static void SetNewItem(DependencyObject d, object value)
        {
            d.SetValue(NewItemProperty, value);
        }

        private static void NewItemChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var collection = new ObservableCollection<object>(((d as ItemsControl)?.Items?.Cast<object>() ?? Array.Empty<object>()).Concat(new[] { e.NewValue }));
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (d is ItemsControl { ItemsSource: { } } itemsControl)
                    itemsControl.ItemsSource = collection;
            });
        }

        #endregion NewItem

        #region Variable

        public static string GetVariable(DependencyObject d)
        {
            return (string)d.GetValue(VariableProperty);
        }

        public static void SetVariable(DependencyObject d, object value)
        {
            d.SetValue(VariableProperty, value);
        }

        public static readonly DependencyProperty VariableProperty = DependencyProperty.RegisterAttached("Variable", typeof(string), typeof(ItemsControlEx), new PropertyMetadata(null, VariableChanged));

        private static void VariableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl? control = d as ItemsControl;
            string arg = (string)e.NewValue;
            if (control?.ItemsSource != null)
                if (control.ItemsSource?.Count() > 0)
                    control.ItemsSource = control.ItemsSource.GetPropertyRefValues<object>(arg);
        }

        #endregion Variable

        #region ItemsSourcEx

        public static IEnumerable GetItemsSourceEx(DependencyObject d)
        {
            return (IEnumerable)d.GetValue(ItemsSourceExProperty);
        }

        public static void SetItemsSourceEx(DependencyObject d, object value)
        {
            d.SetValue(ItemsSourceExProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceExProperty = DependencyProperty.RegisterAttached("ItemsSourceEx", typeof(IEnumerable), typeof(ItemsControlEx), new PropertyMetadata(null, ItemsSourceExChanged));

        private static void ItemsSourceExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl? control = d as ItemsControl;
            IEnumerable? arg = (IEnumerable?)e.NewValue;
            // ReSharper disable once PossibleMultipleEnumeration
            if (arg?.Count() > 0)
                Application.Current.Dispatcher.InvokeAsync(() =>
                (control ?? throw new Exception("sd3 443 ")).SetValue(ItemsSourceProperty, arg.GetPropertyRefValues<object>((string)control.GetValue(VariableProperty)).Cast<IEnumerable<object>>().SelectMany(_s => _s)),
                    System.Windows.Threading.DispatcherPriority.Background, default);
            ;
        }

        #endregion ItemsSourcEx

        #region Filter

        public static string GetFilter(DependencyObject d)
        {
            return (string)d.GetValue(FilterProperty);
        }

        public static void SetFilter(DependencyObject d, object value)
        {
            d.SetValue(FilterProperty, value);
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.RegisterAttached("Filter", typeof(string), typeof(ItemsControlEx), new PropertyMetadata(null, FilterChanged));

        private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl { ItemsSource: { } source })
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(source);
                view.Filter = (obj) => obj.GetType().GetProperties()
                .Where(a => a.PropertyType == e.NewValue.GetType() && a.GetMethod != null)
                .Select(a => a.GetValue(obj))
                .Any(a => a?.ToString()?.Contains((string)e.NewValue) ?? false);
            }
        }

        #endregion Filter

        #region Orientation

        public static Orientation GetOrientation(DependencyObject d)
        {
            return (Orientation)d.GetValue(OrientationProperty);
        }

        public static void SetOrientation(DependencyObject d, object value)
        {
            d.SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(ItemsControlEx),
                new FrameworkPropertyMetadata(OrientationChanged));

        #endregion Orientation

        #region Arrangement

        public static Arrangement GetArrangement(DependencyObject d)
        {
            return (Arrangement)d.GetValue(ArrangementProperty);
        }

        public static void SetArrangement(DependencyObject d, object value)
        {
            d.SetValue(ArrangementProperty, value);
        }

        public static readonly DependencyProperty ArrangementProperty =
            DependencyProperty.RegisterAttached("Arrangement", typeof(Arrangement), typeof(ItemsControlEx),
                new FrameworkPropertyMetadata(ArrangementChanged));

        #endregion Arrangement

        private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
                if (e.NewValue is Orientation orientation)
                {
                    var arrangement = (Arrangement)d.GetValue(ItemsControlEx.ArrangementProperty);
                    LayOutHelper.Changed(itemsControl, orientation, arrangement);
                }
        }

        private static void ArrangementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl)
                if (e.NewValue is Arrangement arrangement)
                {
                    var orientation = (Orientation)d.GetValue(ItemsControlEx.OrientationProperty);
                    LayOutHelper.Changed(itemsControl, orientation, arrangement);
                }
        }
    }
}