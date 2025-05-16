using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Utility.Enums;
using Utility.Helpers;

[assembly: XmlnsDefinition("urn:diagram-designer-ns", "WPF.Connectors")]
namespace Utility.WPF.Trees.Connectors
{
//    public class ConnectionEventArgs(FrameworkElement frameworkElement, Connector connector, RoutedEvent routedEvent) : RoutedEventArgs(routedEvent)
//    {
//        public FrameworkElement FrameworkElement { get; } = frameworkElement;
//        public Connector Connector { get; } = connector;
//    }


    public class ConnectorAdorner : Adorner
    {
        private Point? dragStartPoint = null;
        private FrameworkElement _element;
        private Position2D? _position;
        public Connector connector;
        private readonly FrameworkElement context;
        public static readonly RoutedEvent ConnectionEvent = EventManager.RegisterRoutedEvent(name: "Connection", routingStrategy: RoutingStrategy.Bubble, handlerType: typeof(RoutedEventHandler), ownerType: typeof(ConnectorAdorner));

        public ConnectorAdorner(UIElement adornedElement, FrameworkElement context, Position2D? position = default) : base(adornedElement)
        {
            this.Width = Connector.width; this.Height = Connector.height;
            this._position = position.HasValue ? position : Position2D.All;


            // fired when layout changes
            adornedElement.LayoutUpdated += Connector_LayoutUpdated;
            this.context = context;
        }


        public event RoutedEventHandler Connection
        {
            add { AddHandler(ConnectionEvent, value); }
            remove { RemoveHandler(ConnectionEvent, value); }
        }

        void Connector_LayoutUpdated(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
        //}

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);
 
            if (context != null)
            {
                this.dragStartPoint = new Point?(e.GetPosition(context));

                if (AdornedElement is FrameworkElement frameworkElement && dragStartPoint is Point point)
                {
                    
                    connector = Match(frameworkElement, context, point);
                    if (connector == null)
                        throw new InvalidOperationException();
              

                    //else
                    //    Ex.Connector
                }
                
                //e.Handled = true;
            }
        }

        public Connector Match(FrameworkElement frameworkElement, FrameworkElement canvas, Point point)
        {

            foreach (var x in rects(frameworkElement))
            {
                var pos = x.Rect();
                var rect = new Rect(this.AdornedElement.TransformToAncestor(canvas).Transform(new Point(pos.X, pos.Y)), pos.Size);
                if (rect.Contains(point))
                {
                    return x;
                }
            }
            return null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            // but if mouse button is pressed and start point value is set we do have one
            if (this.dragStartPoint.HasValue)
            {
                this.RaiseEvent(new ConnectionEventArgs(ConnectionEvent, context, e.GetPosition(context), connector, null));
                //var canvas = GetTreeView(this.AdornedElement);
                //if (canvas != null)
                //{

                //    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                //    if (adornerLayer != null)
                //    {
                //        //ConnectionAdorner adorner = new (canvas, connector, this);
                //        //if (adorner != null)
                //        //{
                //        //    adornerLayer.Add(adorner);
                //        //    e.Handled = true;
                //        //}
                //    }
                //}
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (AdornedElement is FrameworkElement frameworkElement)
            {
                foreach (var r in rects(frameworkElement))
                    dc.DrawRectangle(Brushes.LightGray, new Pen(Brushes.LightCyan, 1), r.Rect());
            }
        }


        private IEnumerable<Connector> rects(FrameworkElement frameworkElement)
        {
            foreach (var x in EnumHelper.SeparateFlags(_position.Value))
            {
                switch (x)
                {
                    case Position2D.Top:
                        yield return new TopConnector(frameworkElement);
                        break;
                    case Position2D.Right:
                        yield return new RightConnector(frameworkElement);
                        break;
                    case Position2D.Bottom:
                        yield return new BottomConnector(frameworkElement);
                        break;
                    case Position2D.Left:
                        yield return new LeftConnector(frameworkElement);
                        break;
                }
            }
        }


        public void Remove()
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }      
    } 
}
