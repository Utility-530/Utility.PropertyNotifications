using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Humanizer;
using PixelLab.Common;
using Utility.Interfaces.Exs;

namespace Utility.WPF.Attached
{
    /// <summary>
    /// Provides attached properties and methods for tracking and determining the actual visibility of WPF elements,
    /// considering both their visibility state and whether they are rendered within the bounds of their containing
    /// window   
    /// </summary>
    /// 
    /// <remarks>    
    /// Use the attached properties defined by this class to monitor whether a FrameworkElement is
    /// truly visible to the user, factoring in layout, window bounds, and visibility state. This is useful in scenarios
    /// where an element's logical visibility does not guarantee that it is actually displayed on screen, such as when
    /// it is clipped or outside the window's viewport. The class is intended for use with WPF FrameworkElement
    /// instances and relies on event handlers to update visibility status as layout or window state changes.
    /// </remarks>
    public static class ActualVisibility
    {
        public static readonly DependencyProperty IsWithinWindowBoundsProperty =
            DependencyProperty.RegisterAttached(
                "IsWithinWindowBounds",
                typeof(bool),
                typeof(ActualVisibility),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsWithinWindowBoundsChanged));

        private static void OnIsWithinWindowBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static bool GetIsWithinWindowBounds(DependencyObject d) => (bool)d.GetValue(IsWithinWindowBoundsProperty);

        private static void SetIsWithinWindowBounds(DependencyObject d, bool value) => d.SetValue(IsWithinWindowBoundsProperty, value);


        public static readonly DependencyProperty IsLoadedProperty =
            DependencyProperty.RegisterAttached(
                "IsLoaded",
                typeof(bool),
                typeof(ActualVisibility),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsLoadedChanged));

        private static void OnIsLoadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static bool GetIsLoaded(DependencyObject d) => (bool)d.GetValue(IsLoadedProperty);

        private static void SetIsLoaded(DependencyObject d, bool value) => d.SetValue(IsLoadedProperty, value);


        public static readonly DependencyProperty IsElementLoadedProperty =
            DependencyProperty.RegisterAttached(
                "IsElementLoaded",
                typeof(bool),
                typeof(ActualVisibility),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsElementLoadedChanged));

        private static void OnIsElementLoadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static bool GetIsElementLoaded(DependencyObject d) => (bool)d.GetValue(IsElementLoadedProperty);

        private static void SetIsElementLoaded(DependencyObject d, bool value) => d.SetValue(IsElementLoadedProperty, value);


        public static readonly DependencyProperty TrackStateChangesProperty =
            DependencyProperty.RegisterAttached(
                "TrackStateChanges",
                typeof(bool),
                typeof(ActualVisibility),
                new PropertyMetadata(false, OnTrackStateChangesChanged));

        public static void SetTrackStateChanges(DependencyObject d, bool value) => d.SetValue(TrackStateChangesProperty, value);

        public static bool GetTrackStateChanges(DependencyObject d) => (bool)d.GetValue(TrackStateChangesProperty);


        private static readonly RoutedEventHandler LoadedHandler = (s, e) => OnLoadStateUpdate(s, e);
        private static readonly RoutedEventHandler UnloadedHandler = (s, e) => OnLoadStateUpdate(s, e);

        private static void OnTrackStateChangesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                SetIsElementLoaded(fe, fe.IsLoaded);
                EventHandler LayoutHandler = (s, e) => OnUpdate(fe, e);
                if ((bool)e.NewValue)
                {
                    fe.Loaded += LoadedHandler;
                    fe.Unloaded += UnloadedHandler;
                    fe.LayoutUpdated += LayoutHandler;
                }
                else
                {
                    fe.Loaded -= LoadedHandler;
                    fe.Unloaded -= UnloadedHandler;
                    fe.LayoutUpdated -= LayoutHandler;
                }
            }
        }
        private static void OnLoadStateUpdate(object sender, EventArgs e)
        {
            if (sender is not FrameworkElement fe)
            {
                return;
            }

            SetIsLoaded(fe, fe.IsLoaded);
            return;
        }

        private static void OnUpdate(object sender, EventArgs e)
        {
            return;
            if (sender is not FrameworkElement fe)
            {
                return;
            }
            if (GetIsLoaded(fe) != fe.IsLoaded)
            {
                SetIsLoaded(fe, fe.IsLoaded);
            }

            if (Window.GetWindow(fe) is not Window window)
            {
                SetIsWithinWindowBounds(fe, false);

                return;
            }

            try
            {
                Rect elementRect = fe.TransformToAncestor(window)
                                     .TransformBounds(new Rect(0, 0, fe.ActualWidth, fe.ActualHeight));

                Rect windowRect = new Rect(0, 0, window.ActualWidth, window.ActualHeight);
                if (GetIsWithinWindowBounds(fe) != fe.IsVisible && elementRect.IntersectsWith(windowRect))
                    SetIsWithinWindowBounds(fe, fe.IsVisible && elementRect.IntersectsWith(windowRect));
            }
            catch (InvalidOperationException)
            {
                SetIsWithinWindowBounds(fe, false);
            }
        }
    }
}