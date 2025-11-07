using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Enums;
using Utility.WPF.Helpers;

namespace Utility.WPF.Trees.Connectors
{
    public class HitEventArgs(FrameworkElement frameworkElement, RoutedEvent routedEvent, Position2D? position = null) : RoutedEventArgs(routedEvent)
    {
        public FrameworkElement FrameworkElement { get; } = frameworkElement;
        public Position2D? Position { get; } = position;
    }

    public class ConnectionEventArgs : RoutedEventArgs, INotifyPropertyChanged
    {
        public ConnectionEventArgs(RoutedEvent routedEvent, FrameworkElement context, Point pos, Connector sourceConnector, Connector sinkConnector, bool isComplete = false) : base(routedEvent)
        {
            Context = context;
            Location = pos;
            SourceConnector = sourceConnector;
            SinkConnector = sinkConnector;
            IsComplete = isComplete;
        }

        public FrameworkElement Context { get; }
        public Point Location { get; }
        public Connector SourceConnector { get; }
        public Connector SinkConnector { get; }
        public bool IsComplete { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Handle(bool accept)
        {
            Handled = accept;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Handled)));
        }
    }

    public class ConnectionAdorner : Adorner
    {
        public static readonly RoutedEvent HitEvent = EventManager.RegisterRoutedEvent(name: "Hit", routingStrategy: RoutingStrategy.Bubble, handlerType: typeof(RoutedEventHandler), ownerType: typeof(ConnectionAdorner));
        public static readonly RoutedEvent ConnectionMadeEvent = EventManager.RegisterRoutedEvent(name: "ConnectionMade", routingStrategy: RoutingStrategy.Bubble, handlerType: typeof(RoutedEventHandler), ownerType: typeof(ConnectionAdorner));

        private PathGeometry pathGeometry;
        private FrameworkElement treeView;
        private readonly ConnectorAdorner sourceConnectorAdorner;
        private Pen drawingPen;
        private FrameworkElement hitItem = null;
        private bool isComplete;
        private Connector sourceConnector, sinkConnector;

        public ConnectionAdorner(FrameworkElement treeView, Connector sourceConnector, ConnectorAdorner sourceConnectorAdorner)
            : base(treeView)
        {
            this.treeView = treeView;
            this.sourceConnector = sourceConnector;
            this.sourceConnectorAdorner = sourceConnectorAdorner;
            drawingPen = new Pen(Brushes.LightSlateGray, 1);
            drawingPen.LineJoin = PenLineJoin.Round;
            Cursor = Cursors.Cross;
        }

        public ConnectionEventArgs Args { get; set; }

        public event RoutedEventHandler Hit
        {
            add { AddHandler(HitEvent, value); }
            remove { RemoveHandler(HitEvent, value); }
        }

        public event RoutedEventHandler ConnectionMade
        {
            add { AddHandler(ConnectionMadeEvent, value); }
            remove { RemoveHandler(ConnectionMadeEvent, value); }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (hitItem != null)
            {
                AdornerLayer _adornerLayer = AdornerLayer.GetAdornerLayer(hitItem);
                if (_adornerLayer != null)
                {
                    if (_adornerLayer.GetAdorners(hitItem) is Adorner[] adornersOfStackPanel)
                    {
                        var hitPoint = e.GetPosition(this);

                        foreach (var adorner in adornersOfStackPanel)
                        {
                            if (adorner is ConnectorAdorner connAdorner)
                            {
                                //_tuple = connAdorner.Match(this.hitItem, designerCanvas, hitPoint);
                                var hitConnector = connAdorner;

                                sinkConnector = hitConnector.Match(hitItem, treeView, hitPoint);
                                if (sinkConnector is not null)
                                {
                                    var pos = hitItem.TransformToAncestor(treeView).Transform(sinkConnector.Rect().Centre());
                                    //Ex.ConnectionMade(treeView, pos, sourceConnector, sinkConnector);
                                    Args = new ConnectionEventArgs(ConnectionMadeEvent, treeView, pos, sourceConnector, sinkConnector, true);
                                    RaiseEvent(Args);
                                }
                            }
                        }
                    }
                }
            }

            if (sinkConnector != null)
            {
                update();
                sourceConnector.element.SizeChanged += (s, _e) =>
                {
                    update();
                };
                sinkConnector.element.SizeChanged += (s, _e) =>
                {
                    update();
                };

                sourceConnector.element.IsVisibleChanged += (s, _e) =>
                {
                    update();
                };

                sinkConnector.element.IsVisibleChanged += (s, _e) =>
                {
                    update();
                };

                sourceConnectorAdorner.Remove();
                IsHitTestVisible = false;
                isComplete = true;
            }
            else
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(treeView);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this);
                }
            }
            InvalidateVisual();

            if (IsMouseCaptured)
                ReleaseMouseCapture();
        }

        private void update()
        {
            pathGeometry = GetPathGeometry(sourceCentre(), sinkCentre());
            InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            if (!IsMouseCaptured)
                CaptureMouse();

            var sinkCentre = e.GetPosition(this);
            HitTesting(sinkCentre);
            pathGeometry = GetPathGeometry(sourceCentre(), sinkCentre);
            InvalidateVisual();
            //}
            //else
            //{
            //    if (IsMouseCaptured) ReleaseMouseCapture();
            //}
        }

        private Point sourceCentre()
        {
            return sourceConnector.element.TransformToAncestor(treeView).Transform(sourceConnector.Rect().Centre());
        }

        private Point sinkCentre()
        {
            return sinkConnector.element.TransformToAncestor(treeView).Transform(sinkConnector.Rect().Centre());
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (sinkConnector != null && sinkConnector.element.IsVisible == false || sourceConnector.element.IsVisible == false)
                return;

            dc.DrawGeometry(null, drawingPen, pathGeometry);

            if (sinkConnector != null)
                dc.DrawEllipse(Brushes.LightBlue, new Pen(Brushes.Black, 1), sinkCentre(), 2, 2);

            if (sourceConnector != null)
                dc.DrawEllipse(Brushes.Pink, new Pen(Brushes.Black, 1), sourceCentre(), 2, 2);

            // without a background the OnMouseMove event would not be fired
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            if (isComplete == false)
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }

        private PathGeometry GetPathGeometry(Point sourcePosition, Point sinkPosition)
        {
            PathGeometry geometry = new();

            Position2D targetOrientation;
            if (sinkConnector != null)
                targetOrientation = sinkConnector.Orientation;
            else
                targetOrientation = Position2D.None;

            List<Point> pathPoints = StraightPathFinder.GetConnectionLine(sourcePosition, sinkPosition, targetOrientation);

            if (pathPoints.Count > 0)
            {
                PathFigure figure = new PathFigure
                {
                    StartPoint = pathPoints[0]
                };
                pathPoints.Remove(pathPoints[0]);
                figure.Segments.Add(new PolyLineSegment(pathPoints, true));
                geometry.Figures.Add(figure);
            }

            return geometry;
        }

        private void HitTesting(Point hitPoint)
        {
            DependencyObject hitObject = treeView.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                   hitObject != sourceConnectorAdorner.AdornedElement &&
                   hitObject.GetType() != typeof(Canvas))
            {
                if (hitObject is ConnectorAdorner)
                    return;

                if (hitObject is TreeViewItem tvitem)
                {
                    Debug.WriteLine(hitObject.GetType().Name);
                }
                //if (hitObject is DesignerItem ditem)
                //{
                //    hitItem = ditem;
                //    add(ditem, Position.All);
                //    return;

                //    //if (!hitConnectorFlag)
                //    //    HitConnector = null;

                //}
                if (hitObject is FrameworkElement item)
                {
                    if (item.GetValue(Ex.IsConnectableProperty) is true)
                    {
                        var pos = (Position2D)item.GetValue(Ex.PositionProperty);

                        if (hitItem != null)
                            this.RaiseEvent(new HitEventArgs(hitItem, HitEvent));
                        //Ex.Remove(hitItem);
                        hitItem = item;
                        this.RaiseEvent(new HitEventArgs(item, HitEvent, pos));
                        //Ex.Add(item, pos);
                        return;
                    }
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }
            if (hitItem != null)
            {
                this.RaiseEvent(new HitEventArgs(hitItem, HitEvent));
                //Ex.Remove(hitItem);
                hitItem = null;
            }
        }
    }
}