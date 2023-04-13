using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Utility.WPF.Controls.Dragablz
{
    public class DragablzVerticalItemsControl : DragablzExItemsControl
    {
        //object number;
        //double start = 0;
        //private DeleteAdorner adorner;
        //private AdornerLayer layer;

        static DragablzVerticalItemsControl()
        {
            //DragablzItemsControl.Is.OverrideMetadata(typeof(DragablzVerticalItemsControl), new PropertyMetadata(true))
        }

        public DragablzVerticalItemsControl()
        {
            MouseDown += DragablzVerticalItemsControl_MouseDown;
            var VerticalPositionMonitor = new VerticalPositionMonitor();
            VerticalPositionMonitor.OrderChanged += VerticalPositionMonitor_OrderChanged;
            VerticalPositionMonitor.LocationChanged += VerticalPositionMonitor_LocationChanged;
            PositionMonitor = VerticalPositionMonitor;
            var customOrganiser = new CustomOrganiser();
            customOrganiser.DragCompleted += CustomOrganiser_DragCompleted;
            ItemsOrganiser = customOrganiser;
        }

        private void CustomOrganiser_DragCompleted()
        {
            // if (adorner != null)
            //{
            //MessageBox.Show("Item deleted");
            // }
        }

        private void VerticalPositionMonitor_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Location.Y);
            //if (number != e.Item)
            //{
            //    number = e.Item;
            //    start = e.Location.Y;
            //}
            //if (Math.Abs(e.Location.Y - start) > 50)
            //{
            //    if (adorner == null)
            //    {
            //        layer ??= AdornerLayer.GetAdornerLayer(this);
            //        adorner = new DeleteAdorner(this);
            //        layer.Add(adorner);
            //    }
            //    return;
            //}
            //adorner = null;
            //layer ??= AdornerLayer.GetAdornerLayer(this);
            //Adorner[] toRemoveArray = layer.GetAdorners(this);
            //if (toRemoveArray != null)
            //    foreach (Adorner a in toRemoveArray)
            //    {
            //        layer.Remove(a);
            //    }
        }

        private void VerticalPositionMonitor_OrderChanged(object sender, OrderChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void DragablzVerticalItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

                var width = itemsControl
                    .Items
                    .OfType<object>()
                    .Select(o =>
                    {
                        var container = itemsControl.ItemContainerGenerator.ContainerFromItem(o);
                        return container;
                    })
                    .OfType<FrameworkElement>()
                    .Sum(a => a.ActualWidth);

                drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.White, 1),
                new Rect(new Point(width + 150, 0), new Size(DesiredSize.Height, DesiredSize.Height)));
                //new Rect(new Point(10, DesiredSize.Height - 40), new Size(DesiredSize.Width - 20, 50)));
                base.OnRender(drawingContext);
            }
        }

        private class CustomOrganiser : VerticalOrganiser
        {
            public event Action DragCompleted;

            public override void OrganiseOnDragCompleted(DragablzItemsControl requestor, Size measureBounds, IEnumerable<DragablzItem> siblingItems, DragablzItem dragItem)
            {
                DragCompleted?.Invoke();
                base.OrganiseOnDragCompleted(requestor, measureBounds, siblingItems, dragItem);
            }
        }
    }
}