using DryIoc;
using Fasterflect;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Utility.Enums;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.WPF.Reactives;

namespace Utility.WPF.Trees.Connectors
{

    public class ConnectionEngine
    {
        ReplaySubject<RoutedEventArgs> _connectionMadeSubject = new(1);
        bool flag;

        public IObservable<RoutedEventArgs> Watch(Position2D pos, FrameworkElement _element)
        {
            var disposable = _element
                .Observe(UIElement.IsMouseOverProperty)
                .Subscribe(x =>
                {
                    if (_element.IsMouseOver is true && flag == false)
                    {
                        var ad = Add(_element, pos);
                    }
                    else
                        Remove(_element);
                });

            return _connectionMadeSubject;
        }


        public ConnectorAdorner Add(UIElement _element, Position2D? position = Position2D.All)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_element);
            if (adornerLayer != null)
            {
                //remove(_element);
                var canvas = GetTreeView(_element);
                var adorner = new ConnectorAdorner(_element, canvas, position);

                ConnectionAdorner connectionAdorner = null;

                adorner.PreviewMouseUp += (s, e) =>
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                    if (adornerLayer != null)
                    {
                        var pos = e.GetPosition(canvas);
                        var callBack = new ConnectionEventArgs(Ex.ConnectionMadeEvent, canvas, pos, adorner.connector, null);

                        callBack.WithChangesTo(a => a.Handled)
                        .WhereTrue()
                        .Subscribe(x =>
                        {
                            flag = false;

                            if (connectionAdorner != null)
                            {

                                _connectionMadeSubject.OnNext(connectionAdorner.Args);
                            }
                            else
                            {
                                connectionAdorner = new(canvas, adorner.connector, adorner);

                                connectionAdorner.Hit += (s, e) =>
                                {
                                    if (e is HitEventArgs args)
                                    {
                                        if (args.Position.HasValue)
                                        {
                                            Add(args.FrameworkElement, args.Position);

                                        }
                                        else
                                        {
                                            Remove(args.FrameworkElement);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("3 4w");
                                    }
                                };

                                connectionAdorner.ConnectionMade += (s, _e) =>
                                {
                                    if (_e is ConnectionEventArgs args)
                                    {
                                        flag = true;
                                        var pos = e.GetPosition(canvas);
                                        //var sdd = new ConnectionMadeEventArgs(null, args.Context, args.Pos, args.SourceConnector, args.SinkConnector);
                                        var sdd = args;
                                        callBack.WithChangesTo(a => a.Handled)
                                        .WhereTrue()
                                        .Subscribe(x =>
                                        {
                                            _connectionMadeSubject.OnNext(args);
                                        });
                                        _connectionMadeSubject.OnNext(sdd);
                                    }

                                };
                                adornerLayer.Add(connectionAdorner);
                                e.Handled = true;

                            }
                        });
                        _connectionMadeSubject.OnNext(callBack);
                    }
                };

                adornerLayer.Add(adorner);
                _element.RaiseEvent(new RoutedEventArgs(Ex.CleanEvent));
                return adorner;

            }
            return null;


        }

        public void Remove(UIElement _element)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_element);
            if (adornerLayer != null)
            {
                if (adornerLayer.GetAdorners(_element) is Adorner[] adornersOfStackPanel)

                    foreach (var adorner in adornersOfStackPanel)
                    {
                        if (adorner?.IsMouseOver != false)
                        {
                            adorner.Observe(Adorner.IsMouseOverProperty).Subscribe(a =>
                            {
                                if (adorner.IsMouseOver == false)
                                {
                                    adornerLayer.Remove(adorner);

                                }
                            });
                        }
                        else
                        {
                            adornerLayer.Remove(adorner);

                        }
                    }
            }
        }

        //public IDisposable Subscribe(IObserver<ConnectionMadeEventArgs> observer)
        //{
        //    return _connectionMadeSubject.Subscribe(observer);  
        //}

        static FrameworkElement GetTreeView(DependencyObject element)
        {
            while (element != null && element is not TreeView)
                element = VisualTreeHelper.GetParent(element);

            return element as TreeView;
        }

        public static ConnectionEngine Instance { get; } = new();
        public static ConnectionEngine Create() => new();
    }

}
