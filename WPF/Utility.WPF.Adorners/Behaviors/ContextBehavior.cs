using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Grid grid = new Grid() { Height = 16, Width = 16, Background = Brushes.Transparent };

            grid.Children.Add(CreatePath());

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

        private Path CreatePath()
        {
            double CapDiameter = 2;
            Point ellipseCenter1 = new(0, 0);
            Point ellipseCenter2 = new(16, 0);
            Point ellipseCenter3 = new(32, 0);
            var ellipse1 = new EllipseGeometry(ellipseCenter1, CapDiameter, CapDiameter);
            var ellipse2 = new EllipseGeometry(ellipseCenter2, CapDiameter, CapDiameter);
            var ellipse3 = new EllipseGeometry(ellipseCenter3, CapDiameter, CapDiameter);
            GeometryGroup combined = new GeometryGroup();
            combined.Children.Add(ellipse1);
            combined.Children.Add(ellipse2);
            combined.Children.Add(ellipse3);

            //string sData = "M19.375 36.7818V100.625C19.375 102.834 21.1659 104.625 23.375 104.625H87.2181C90.7818 104.625 92.5664 100.316 90.0466 97.7966L26.2034 33.9534C23.6836 31.4336 19.375 33.2182 19.375 36.7818Z";
            Path path = new Path() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.Black), StrokeThickness = 2, Stretch = Stretch.Uniform };
            //string sData = "M 250,40 L200,20 L200,60 Z";
            //var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            //path.Data = (Geometry)converter.ConvertFrom(sData);
            path.Data = combined;
            return path;

        }


    }
}

