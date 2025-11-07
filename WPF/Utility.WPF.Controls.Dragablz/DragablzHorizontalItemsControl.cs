using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Dragablz;

namespace Utility.WPF.Controls.Dragablz
{
    public class DragablzHorizontalItemsControl : DragablzExItemsControl
    {
        private object number;
        private double start = 0;
        private DeleteAdorner adorner;
        private AdornerLayer layer;

        public DragablzHorizontalItemsControl()
        {
            MouseDown += DragablzHorizontalItemsControl_MouseDown;
            var horizontalPositionMonitor = new HorizontalPositionMonitor();
            horizontalPositionMonitor.OrderChanged += HorizontalPositionMonitor_OrderChanged;
            horizontalPositionMonitor.LocationChanged += HorizontalPositionMonitor_LocationChanged;
            PositionMonitor = horizontalPositionMonitor;
            var customOrganiser = new CustomOrganiser();

            customOrganiser.DragCompleted += CustomOrganiser_DragCompleted;
            ItemsOrganiser = customOrganiser;
        }

        private void CustomOrganiser_DragCompleted()
        {
            UIElement uiElement = (UIElement)ItemContainerGenerator.ContainerFromItem(lastItem);

            if (adorner == null)
            {
                return;
            }
            Rect? adornerRect = (Rect?)GetPrivatePropertyValue(adorner, "VisualContentBounds");

            if (adornerRect == default)
            {
                return;
            }
            var val = adornerRect!.Value;
            System.Diagnostics.Debug.WriteLine(val.Left - lastX - uiElement.DesiredSize.Width);

            if (adorner != null && lastX + uiElement.DesiredSize.Width > val.Left && lastX < val.Left + val.Width + uiElement.DesiredSize.Width)
            {
                MessageBox.Show("Item deleted");
            }

            static object GetPrivatePropertyValue(object obj, string propName)
            {
                if (obj == null)
                    throw new ArgumentNullException("obj");
                if (propName == null)
                    throw new ArgumentNullException("propName");
                PropertyInfo pi = null;

                Type t = obj.GetType();
                pi = t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (pi == null)
                    throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));

                return pi.GetValue(obj, null);
            }
        }

        private object lastItem;
        private double lastX;
        private const int arbitraryOffset = 100;

        private void HorizontalPositionMonitor_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Location.X);
            lastItem = e.Item;
            lastX = e.Location.X;
            if (number != e.Item)
            {
                number = e.Item;
                start = e.Location.X;
            }
            if (e.Location.X - start > ActualHeight)
            {
                if (adorner == null)
                {
                    layer ??= AdornerLayer.GetAdornerLayer(this);
                    adorner = new DeleteAdorner(this);
                    layer.Add(adorner);
                }
                return;
            }
            if (adorner == null)
                return;

            adorner = null;
            layer ??= AdornerLayer.GetAdornerLayer(this);
            Adorner[] toRemoveArray = layer.GetAdorners(this);
            if (toRemoveArray != null)
                foreach (Adorner a in toRemoveArray)
                {
                    layer.Remove(a);
                }
        }

        private void HorizontalPositionMonitor_OrderChanged(object sender, OrderChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void DragablzHorizontalItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private class DeleteAdorner : Adorner
        {
            public DeleteAdorner(ItemsControl adornerElement) : base(adornerElement)
            {
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                if (AdornedElement is not ItemsControl itemsControl)
                    return;

                GradientStop whiteGradientStop = new GradientStop
                {
                    Color = Colors.Transparent,
                    Offset = 0.2
                };
                GradientStop redGradientStop = new GradientStop
                {
                    Color = Colors.Red,
                    Offset = 0.8
                };
                var fiveColorLGB = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };

                fiveColorLGB.GradientStops.Add(whiteGradientStop);
                fiveColorLGB.GradientStops.Add(redGradientStop);
                fiveColorLGB.Opacity = 0.5;

                drawingContext.DrawRectangle(fiveColorLGB, new Pen(Brushes.Transparent, 0),
                new Rect(new Point(itemsControl.ActualWidth - DesiredSize.Height, 0), new Size(DesiredSize.Height, DesiredSize.Height)));
                base.OnRender(drawingContext);
            }
        }

        private class CustomOrganiser : HorizontalOrganiser
        {
            public event Action DragCompleted;

            public override void OrganiseOnDragCompleted(DragablzItemsControl requestor, Size measureBounds, IEnumerable<DragablzItem> siblingItems, DragablzItem dragItem)
            {
                DragCompleted?.Invoke();
                base.OrganiseOnDragCompleted(requestor, measureBounds, siblingItems, dragItem);
            }

            public override Point ConstrainLocation(DragablzItemsControl requestor, Size measureBounds, Point itemCurrentLocation, Size itemCurrentSize, Point itemDesiredLocation, Size itemDesiredSize)
            {
                return base.ConstrainLocation(requestor, new Size(measureBounds.Width + 10000, measureBounds.Height), itemCurrentLocation, itemCurrentSize, itemDesiredLocation, itemDesiredSize);
            }
        }
    }
}