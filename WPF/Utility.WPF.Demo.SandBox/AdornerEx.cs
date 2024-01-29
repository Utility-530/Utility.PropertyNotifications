using AdornerBehavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace Utility.WPF.Adorners.Infrastructure
{
    /// <summary>
    /// <a href="https://www.codeproject.com/Articles/123638/A-Resusable-Attached-Behavior-for-Defining-Adorner">
    /// A behavior that allows a collection of adorners be defined in XAML.</a>
    /// </summary>
    /// taken from
    /// https://github.com/dady8889/AdornerBehavior
    public static class AdornerEx
    {
        /// <summary>
        /// Apply this on the target element you want to adorn
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(AdornerEx),
                new FrameworkPropertyMetadata(true, OnPropertyChanged));

        /// <summary>
        /// The internal encapsulating adorner
        /// </summary>
        public static readonly DependencyProperty AdornerProperty =
            DependencyProperty.RegisterAttached("Adorner", typeof(FrameworkElementAdorner), typeof(AdornerEx), new PropertyMetadata(Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (e.NewValue is SettingsAdorner)
            //{
            //    d.SetValue(AdornerEx.IsEnabledProperty, true);
            //}
        }

        /// <summary>
        /// Used in XAML to define the collection of adorners.
        /// </summary>
        /// <remarks>
        /// The Get property of this DependencyProperty is basically a constructor for this behavior.
        /// https://stackoverflow.com/questions/1448899/attached-property-of-type-list
        /// </remarks>
        public static readonly DependencyProperty AdornersProperty =
            DependencyProperty.RegisterAttached("AdornersInternal", typeof(AdornerCollection), typeof(AdornerEx),
                new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// X axis offset relative to the specified placement and alignment.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.RegisterAttached("OffsetX", typeof(double), typeof(AdornerEx), new PropertyMetadata(0.0));

        /// <summary>
        /// Y axis offset relative to the specified placement and alignment.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.RegisterAttached("OffsetY", typeof(double), typeof(AdornerEx), new PropertyMetadata(0.0));

        /// <summary>
        /// The horizontal placement of the adorner. Used only on Left or Right horizontally aligned adorners.
        /// </summary>
        public static readonly DependencyProperty HorizontalPlacementProperty =
            DependencyProperty.RegisterAttached("HorizontalPlacement", typeof(AdornerPlacement), typeof(AdornerEx),
                new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        /// <summary>
        /// The vertical placement of the adorner. Used only on Top or Bottom vertically aligned adorners.
        /// </summary>
        public static readonly DependencyProperty VerticalPlacementProperty =
            DependencyProperty.RegisterAttached("VerticalPlacement", typeof(AdornerPlacement), typeof(AdornerEx),
                new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateAdorner((FrameworkElement)d);
        }

        #region Dependency Properties Get And Set

        public static AdornerCollection GetAdorners(this DependencyObject d)
        {
            if (d.GetValue(AdornersProperty) is not AdornerCollection collection)
            {
                var fe = d as FrameworkElement ?? throw new Exception("sdf2 vv");
                collection = new AdornerCollection(fe);
                collection.CollectionChanged += Adorners_CollectionChanged;

                d.SetValue(AdornersProperty, collection);

                fe.DataContextChanged += AdornedFrameworkElement_DataContextChanged;
                fe.Loaded += AdornedFrameworkElement_Loaded;
                fe.Unloaded += AdornedFrameworkElement_Unloaded;
            }
            return collection;

            void AdornedFrameworkElement_Loaded(object sender, RoutedEventArgs args)
            {
                UpdateAdorner((FrameworkElement)sender);
            }

            void AdornedFrameworkElement_Unloaded(object sender, RoutedEventArgs args)
            {
                HideAdorner((FrameworkElement)sender);
            }
        }

        public static void SetAdorners(DependencyObject d, AdornerCollection value)
        {
            d.SetValue(AdornersProperty, value);
        }

        public static bool GetIsEnabled(DependencyObject d)
        {
            return (bool)d.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject d, bool value)
        {
            d.SetValue(IsEnabledProperty, value);
        }

        public static AdornerPlacement GetHorizontalPlacement(DependencyObject d)
        {
            return (AdornerPlacement)d.GetValue(HorizontalPlacementProperty);
        }

        public static void SetHorizontalPlacement(DependencyObject d, AdornerPlacement value)
        {
            d.SetValue(HorizontalPlacementProperty, value);
        }

        public static AdornerPlacement GetVerticalPlacement(DependencyObject d)
        {
            return (AdornerPlacement)d.GetValue(VerticalPlacementProperty);
        }

        public static void SetVerticalPlacement(DependencyObject d, AdornerPlacement value)
        {
            d.SetValue(VerticalPlacementProperty, value);
        }

        public static double GetOffsetX(DependencyObject d)
        {
            return (double)d.GetValue(OffsetXProperty);
        }

        public static void SetOffsetX(DependencyObject d, double value)
        {
            d.SetValue(OffsetXProperty, value);
        }

        public static double GetOffsetY(DependencyObject d)
        {
            return (double)d.GetValue(OffsetYProperty);
        }

        public static void SetOffsetY(DependencyObject d, double value)
        {
            d.SetValue(OffsetYProperty, value);
        }

        public static Adorner GetAdorner(DependencyObject d)
        {
            return (Adorner)d.GetValue(AdornerProperty);
        }

        public static void SetAdorner(DependencyObject d, Adorner value)
        {
            d.SetValue(AdornerProperty, value);
        }


        #endregion Dependency Properties Get And Set

        /// <summary>
        /// Let the adorners in the collection inherit parent DataContext.
        /// </summary>
        private static void AdornedFrameworkElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var fe = sender as FrameworkElement;
            if (fe == null)
                return;

            var adorners = GetAdorners(fe);

            foreach (var adorner in adorners)
                adorner.DataContext = fe.DataContext;
        }

        private static void Adorners_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is not FrameworkElement adornedElement ||
                adornedElement.GetValue(AdornerProperty) is not FrameworkElementAdorner encapsulatingAdorner)
            {
                // The statically typed Adorners in XAML will not pass through this function, because the view is not rendered yet.
                return;
            }

            //System.Diagnostics.Debug.WriteLine($"Action: {e.Action} NewItems: {e.NewItems?.Count ?? 0} OldItems: {e.OldItems?.Count ?? 0}");

            var newItems = e.NewItems?.Cast<FrameworkElement>()?.ToList();
            var oldItems = e.OldItems?.Cast<FrameworkElement>()?.ToList();

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (newItems != null)
                    {
                        foreach (var adorner in newItems)
                            encapsulatingAdorner.ConnectAdorner(adornedElement, adorner);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (oldItems != null)
                    {
                        foreach (var adorner in oldItems)
                            encapsulatingAdorner.DisconnectAdorner(adorner);
                    }
                    break;

                default:
                    break;
            }

            encapsulatingAdorner.InvalidateMeasure();
        }

        /// <summary>
        /// Decide whether to show or hide the adorner collection.
        /// </summary>
        private static void UpdateAdorner(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
                return;

            if (GetIsEnabled(frameworkElement))
                ShowAdorner(frameworkElement);
            else
                HideAdorner(frameworkElement);
        }

        /// <summary>
        /// Create a new encapsulating adorner or reuse existing and connect the children.
        /// </summary>
        private static void ShowAdorner(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
                return;

            var adorners = GetAdorners(frameworkElement);
            if (adorners == null)
                return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(frameworkElement);
            if (adornerLayer == null)
            {

                //frameworkElement.Loaded += (s, e) => AdornerEx.UpdateAdorner(frameworkElement);
                return;
            }
        

            // create new adorner if it doesnt exist
            if (frameworkElement.GetValue(AdornerProperty) is not FrameworkElementAdorner adorner)
            {
                adorner = new FrameworkElementAdorner(adorners, frameworkElement);
                frameworkElement.SetValue(AdornerProperty, adorner);
            }

            BindAdorner(frameworkElement, adorner);

            if (adorner.Parent == null)
            {
                if (adorners.IndexOf(adorner) != -1)
                    return;
                adorner.ConnectChildren(frameworkElement);
                adornerLayer.Add(adorner);
            }
        }

  

        /// <summary>
        /// Detach the encapsulating adorner from the visual tree, including children.
        /// </summary>
        private static void HideAdorner(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
                return;

            if (frameworkElement.GetValue(AdornerProperty) is FrameworkElementAdorner adorner)
            {
                if (AdornerLayer.GetAdornerLayer(frameworkElement) is AdornerLayer al)
                    al.Remove(adorner);
                adorner.DisconnectChildren();
                //frameworkElement.SetValue(AdornerProperty, null);
            }
        }

        /// <summary>
        /// Bind the adorner collection to the encapsulating adorner.
        /// </summary>
        private static void BindAdorner(FrameworkElement frameworkElement, FrameworkElementAdorner adorner)
        {
            if (adorner == null)
                throw new ArgumentNullException(nameof(adorner));

            if (frameworkElement == null)
                throw new ArgumentNullException(nameof(frameworkElement));

            adorner.SetBinding(FrameworkElementAdorner.AdornersProperty,
                new Binding
                {
                    Path = new PropertyPath(AdornersProperty),
                    Mode = BindingMode.OneWay,
                    Source = frameworkElement
                });
        }
    }
}