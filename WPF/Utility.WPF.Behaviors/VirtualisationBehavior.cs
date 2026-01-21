using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;
using Utility.WPF.Helpers;

namespace Utility.WPF.Behaviors
{
    public class VirtualisationBehavior : Behavior<ItemsControl>
    {
        public static readonly DependencyProperty FirstVisibleProperty =
    DependencyProperty.Register(nameof(FirstVisible), typeof(int), typeof(VirtualisationBehavior), new PropertyMetadata(0));



        public static readonly DependencyProperty VisibleCountProperty =
    DependencyProperty.Register(nameof(VisibleCount), typeof(int), typeof(VirtualisationBehavior), new PropertyMetadata(0));


        protected override void OnAttached()
        {
            if (AssociatedObject.IsLoaded)
                loaded(AssociatedObject, null);
            else
                AssociatedObject.Loaded += loaded;
            base.OnAttached();
        }
        private void loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                // Get the VirtualizingStackPanel inside the DataGrid
                var vsp = itemsControl.FindChild<VirtualizingStackPanel>();

                if (vsp == null) return;

                // Track scroll changes
                vsp.LayoutUpdated += (_, _) =>
                {
                    FirstVisible = (int)vsp.VerticalOffset;
                    VisibleCount = (int)vsp.ViewportHeight;
                    //UpdateVisibleRangeFromVSP_Throttled(vsp, dataGrid);
                };
            }
        }

        private DispatcherTimer? throttleTimer;
        private int pendingFirst = -1;
        private int pendingVisible = -1;

        private void UpdateVisibleRangeFromVSP_Throttled(VirtualizingStackPanel vsp, DataGrid grid)
        {
            int firstVisible = (int)vsp.VerticalOffset;
            int visibleCount = (int)vsp.ViewportHeight;

            pendingFirst = firstVisible;
            pendingVisible = visibleCount;

            if (throttleTimer == null)
            {
                throttleTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                throttleTimer.Tick += (_, __) =>
                {
                    throttleTimer.Stop();
                    throttleTimer = null;


                    FirstVisible = pendingFirst;
                    VisibleCount = pendingVisible;

                };
                throttleTimer.Start();
            }
        }

        public int FirstVisible
        {
            get { return (int)GetValue(FirstVisibleProperty); }
            set { SetValue(FirstVisibleProperty, value); }
        }

        public int VisibleCount
        {
            get { return (int)GetValue(VisibleCountProperty); }
            set { SetValue(VisibleCountProperty, value); }
        }


    }
}