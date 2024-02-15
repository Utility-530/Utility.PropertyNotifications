using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Resources;

namespace Utility.WPF.Adorners
{
    public class ContextBehavior : Behavior<FrameworkElement>
    {
        private DispatcherTimer closeAdornerTimer = new DispatcherTimer();

        public double CloseAdornerTimeOut { get; } = 2.0;

        public ContextBehavior()
        {

            closeAdornerTimer.Tick += new EventHandler(closeAdornerTimer_Tick);
            closeAdornerTimer.Interval = TimeSpan.FromSeconds(CloseAdornerTimeOut);
        }

        private void closeAdornerTimer_Tick(object? sender, EventArgs e)
        {
            AssociatedObject.ContextMenu.IsOpen = false;
            closeAdornerTimer.Stop();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            var adorners = AssociatedObject.GetAdorners();
            var element = CreateElement();
            element.HorizontalAlignment = HorizontalAlignment.Right;
            element.VerticalAlignment = VerticalAlignment.Top;
            element.Margin = new Thickness(0, 0, 2, 0);
            adorners.Add(element);

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            //if (AssociatedObject.ContextMenu != null)
                //AssociatedObject.ContextMenu.Mouse += ContextMenu_MouseLeave;
            //AssociatedObject.ContextMenu.IsMouseCapturedChanged += ContextMenu_IsMouseCapturedChanged; ;

            //void ContextMenu_MouseLeave(object sender, MouseEventArgs e)
            //{

            //}
        }

        //private void ContextMenu_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if(e.NewValue is false)
        //    {
        //        if (closeAdornerTimer.IsEnabled == true)
        //            closeAdornerTimer.Stop();
        //        closeAdornerTimer.Start();
        //    }
        //}

        //    var descriptor = DependencyPropertyDescriptor.FromProperty(AdornerEx.AdornerProperty, AssociatedObject.GetType());
        //    descriptor.AddValueChanged(AssociatedObject, OnAdornerChanged);
        //}

        //private void OnAdornerChanged(object? sender, EventArgs e)
        //{
        //    var adorners = AssociatedObject.GetAdorners();
        //    adorners.Add(CreateElement());
        //}

        private FrameworkElement CreateElement()
        {

            //https://stackoverflow.com/questions/5664441/making-a-control-visible-to-hit-testing-but-transparent-to-dragdrop
            Grid grid = new () { Height = 16, Width = 16, Background = Brushes.Transparent };

            Path path = new Path() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.Black), Data= Shapes.Ellipse, StrokeThickness = 2, Stretch = Stretch.Uniform };
            grid.Children.Add(path);

            grid.MouseEnter += Grid_MouseEnter;
            grid.MouseLeave += Grid_MouseLeave;

            return grid;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {

            if (AssociatedObject.ContextMenu != null)
            {
                if (AssociatedObject.ContextMenu.IsMouseOver == false)
                    closeAdornerTimer.Start();
            }

        }



        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (AssociatedObject.ContextMenu != null)
                AssociatedObject.ContextMenu.IsOpen = true;
        }

      

    }
}

