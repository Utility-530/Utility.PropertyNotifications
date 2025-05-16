using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Utility.Enums;

namespace Utility.WPF.Trees.Connectors
{
    public class Ex
    {

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(Position2D), typeof(Ex), new PropertyMetadata(position_changed));
        public static readonly DependencyProperty IsConnectableProperty =
    DependencyProperty.RegisterAttached("IsConnectable", typeof(bool), typeof(Ex), new PropertyMetadata(is_connectable_changed));

        public static readonly DependencyProperty UsesCustomEventHandlerProperty =
    DependencyProperty.RegisterAttached("UsesCustomEventHandler", typeof(bool), typeof(Ex), new PropertyMetadata());
        public static readonly RoutedEvent ConnectionMadeEvent = EventManager.RegisterRoutedEvent("ConnectionMade", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Ex));
        public static readonly RoutedEvent CleanEvent = EventManager.RegisterRoutedEvent("Clean", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Ex));


        public static void AddConnectionMadeHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is not UIElement uiElement)
                return;
            uiElement.AddHandler(ConnectionMadeEvent, handler);
        }
        public static void RemoveConnectionMadeHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is not UIElement uiElement)
                return;
            uiElement.RemoveHandler(ConnectionMadeEvent, handler);
        }

        public static void AddCleanHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is not UIElement uiElement)
                return;

            uiElement.AddHandler(CleanEvent, handler);
        }

        public static void RemoveCleanHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
        {
            if (dependencyObject is not UIElement uiElement)
                return;

            uiElement.RemoveHandler(CleanEvent, handler);
        }


        public static Position2D GetPosition(UIElement element)
        {
            return (Position2D)element.GetValue(PositionProperty);
        }

        public static void SetPosition(UIElement element, Position2D value)
        {
            element.SetValue(PositionProperty, value);
        }

        public static bool GetIsConnectable(UIElement element)
        {
            return (bool)element.GetValue(IsConnectableProperty);
        }

        public static void SetIsConnectable(UIElement element, bool value)
        {
            element.SetValue(IsConnectableProperty, value);
        }


        public static bool GetUsesCustomEventHandler(UIElement element)
        {
            return (bool)element.GetValue(UsesCustomEventHandlerProperty);
        }

        public static void SetUsesCustomEventHandler(UIElement element, bool value)
        {
            element.SetValue(UsesCustomEventHandlerProperty, value);
        }


        private static void position_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Position2D p)
            {
                if (d is FrameworkElement element)
                {
                    ConnectionEngine
                        .Create()
                        .Watch(p, element)
                        .Subscribe(_a =>
                        {
                            if (_a is ConnectionEventArgs { IsComplete: { } isComplete } args)
                            {
                                if (isComplete == false)
                                    ConnectionInProgress(args);
                                else if (isComplete)
                                    ConnectionMade(args.Context, args.Location, args.SourceConnector, args.SinkConnector);
                                else
                                    throw new InvalidOperationException();
                            }
                            else
                                throw new InvalidOperationException();
                        });
                    element.SetValue(IsConnectableProperty, true);
                }
            }
        }

        private static void is_connectable_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                if (d is FrameworkElement element)
                {
                    if (element.GetValue(PositionProperty) is null)
                    {
                        ConnectionEngine
                            .Create()
                            .Watch(Position2D.All, element)
                            .Subscribe(_a =>
                            {
                                if (_a is ConnectionEventArgs { IsComplete: { } isComplete } args)
                                {
                                    if (isComplete == false)
                                        ConnectionInProgress(args);
                                    else if (isComplete)
                                        ConnectionMade(args.Context, args.Location, args.SourceConnector, args.SinkConnector);
                                    else
                                        throw new InvalidOperationException();
                                }
                                else
                                    throw new InvalidOperationException();
                            });
                    }

                }
            }
        }

        internal static void ConnectionInProgress(ConnectionEventArgs args)
        {
            if (GetUsesCustomEventHandler(args.Context))
            {
                args.Context.RaiseEvent(args);
            }
            else
            {
                args.Handle(true);
            }
        }

        internal static void ConnectionMade(FrameworkElement _element, Point pos, Connector sourceConnector, Connector sinkConnector)
        {
            if (GetUsesCustomEventHandler(_element))
                _element.RaiseEvent(new ConnectionEventArgs(Ex.ConnectionMadeEvent, _element, pos, sourceConnector, sinkConnector, true));
            else
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_element);
                var adorner = new ComboAdorner(_element, pos);
                adorner.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((s, e) =>
                {
                    var x = adornerLayer.GetAdorners(_element);
                    adornerLayer.Remove(adorner);
                }));
                adornerLayer.Add(adorner);
            }
        }
    }

}
