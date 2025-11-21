using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Utility.WPF.SandBox
{
    /// <summary>
    /// Interaction logic for TrackWindow.xaml
    /// </summary>
    public partial class TrackWindow : Window
    {
        public TrackWindow()
        {
            InitializeComponent();
        }
    }


    public static class ActualVisibility
    {
        public static readonly DependencyProperty IsActuallyVisibleProperty =
            DependencyProperty.RegisterAttached(
                "IsActuallyVisible",
                typeof(bool),
                typeof(ActualVisibility),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsActuallyVisibleChanged));

        private static void OnIsActuallyVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }

        public static bool GetIsActuallyVisible(DependencyObject d) =>
            (bool)d.GetValue(IsActuallyVisibleProperty);

        private static void SetIsActuallyVisible(DependencyObject d, bool value) =>
            d.SetValue(IsActuallyVisibleProperty, value);

        public static readonly DependencyProperty TrackVisibilityProperty =
            DependencyProperty.RegisterAttached(
                "TrackVisibility",
                typeof(bool),
                typeof(ActualVisibility),
                new PropertyMetadata(false, OnTrackVisibilityChanged));

        public static void SetTrackVisibility(DependencyObject d, bool value) =>
            d.SetValue(TrackVisibilityProperty, value);

        public static bool GetTrackVisibility(DependencyObject d) =>
            (bool)d.GetValue(TrackVisibilityProperty);

        private static readonly RoutedEventHandler LoadedHandler = (s, e) => OnUpdate(s, e);
        private static readonly RoutedEventHandler UnloadedHandler = (s, e) => OnUpdate(s, e);
        private static readonly DependencyPropertyChangedEventHandler VisibleHandler = (s, e) => OnUpdate(s, EventArgs.Empty);
        private static readonly EventHandler LayoutHandler = (s, e) => OnUpdate(s, e);

        private static void OnTrackVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                if ((bool)e.NewValue)
                {
                    fe.Loaded += LoadedHandler;
                    fe.Unloaded += UnloadedHandler;
                    fe.IsVisibleChanged +=VisibleHandler;
                    fe.LayoutUpdated += LayoutHandler;
                }
                else
                {
                    fe.Loaded -= LoadedHandler;
                    fe.Unloaded -= UnloadedHandler;
                    fe.IsVisibleChanged -= VisibleHandler;
                    fe.LayoutUpdated -= LayoutHandler;
                }
            }
        }
        private static void OnUpdate(object sender, EventArgs e)
        {
            if (sender is not FrameworkElement fe)
            {
                return;
            }
            if (!fe.IsLoaded)
            {
                SetIsActuallyVisible(fe, false);
                return;
            }

            if (Window.GetWindow(fe) is not Window window)
            {
                SetIsActuallyVisible(fe, false);
                return;
            }

            try
            {
                Rect elementRect = fe.TransformToAncestor(window)
                                     .TransformBounds(new Rect(0, 0, fe.ActualWidth, fe.ActualHeight));

                Rect windowRect = new Rect(0, 0, window.ActualWidth, window.ActualHeight);

                SetIsActuallyVisible(fe, fe.IsVisible && elementRect.IntersectsWith(windowRect));
            }
            catch (InvalidOperationException)
            {
                SetIsActuallyVisible(fe, false);
            }
        }
    }

}
